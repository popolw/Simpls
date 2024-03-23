using StackExchange.Redis;
using System.Collections.Concurrent;

namespace Simpls
{
	public class MachineUnAvailable
	{

		public MachineUnAvailable(string machine)
		{
			this.Machine = machine;
		}
		public string Machine { get; private set; }
	}

	/// <summary>
	/// 机台可用性检测
	/// </summary>
	public class MachineAvailableChecker
	{

		private readonly string _machine;
		private readonly Func<string,bool> _onCheck;
		private readonly Func<MachineUnAvailable, bool> _onUnAvailable;
		private readonly CancellationTokenSource _source = new CancellationTokenSource();

		public MachineAvailableChecker(string machine, Func<string, bool> onCheck, Func<MachineUnAvailable, bool> onUnAvailable)
		{
			this._machine = machine;
			this._onCheck = onCheck;
			this._onUnAvailable = onUnAvailable;
			Task.Run(this.Check, _source.Token);
		}


		/// <summary>
		/// 是否是可用的
		/// </summary>
		public bool Available { get; private set; }


		private void Check()
		{
			while (!_source.Token.IsCancellationRequested)
			{
				var free = this._onCheck(_machine);
				if (!free)
				{
					this.Available = false;
					if (this._onUnAvailable(new MachineUnAvailable(this._machine))) break;
				}
				Task.Delay(TimeSpan.FromSeconds(5)).Wait();
			}
		}

		public void Complete()
		{
			this._source.Cancel();
		}
	}

	/// <summary>
	///机台检测管理器
	/// </summary>
	public class MachineAvailableCheckerManager
	{
		private readonly IDatabase _cache;
		private readonly Func<string, bool> _onFilter;
		private readonly Func<string, bool> _onCheck;
		private readonly Func<MachineUnAvailable, bool> _onUnAvailable;
		private readonly ConcurrentDictionary<string, MachineAvailableChecker> _checkers = new ConcurrentDictionary<string, MachineAvailableChecker>();
		private static readonly string MACHINE_CHECKER = "TI_PROBE:MACHINE_CHECKER";

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="cache">数据存储对象</param>
		/// <param name="onFilter">
		/// 验证是否可以生成检查器<see cref="MachineAvailableChecker"/>
		/// 不是所有情况下都需要检测机台的可用性，只有在往机台送lot的情况下需要检测
		/// 这里检测是为了防止中途运输过程中AE维修机台而没有变更机台状态，减少IAR撞机的可能性
		/// </param>
		/// <param name="onCheck">一个用以检测机台状态的委托</param>
		/// <param name="onUnAvailable">一个用于处理如果机台不可用的回调</param>
		public MachineAvailableCheckerManager(
			IDatabase cache,
			Func<string, bool> onFilter,
			Func<string, bool> onCheck,
			Func<MachineUnAvailable, bool> onUnAvailable)
		{
			this._cache = cache;
			this._onFilter = onFilter;
			this._onCheck = onCheck;
			this._onUnAvailable = onUnAvailable;

			this.Check();
		}



		private void Check()
		{
			var machine = this._cache.StringGet(MACHINE_CHECKER);
			//机台都已TE开头
			if (!string.IsNullOrWhiteSpace(machine) && this._onFilter(machine))
			{
				_checkers.GetOrAdd(machine, key => new MachineAvailableChecker(machine, this._onCheck, this._onUnAvailable));
			}
		}


		/// <summary>
		/// 设置要检测状态的机台名称
		/// </summary>
		/// <param name="machine">机台名称</param>
		public void Set(string machine)
		{
			//机台都已TE开头
			if (machine.StartsWith("TE") && this._onFilter(machine))
			{
				_cache.StringSet(MACHINE_CHECKER, machine);
				if (!_checkers.ContainsKey(machine))
				{
					 _checkers.GetOrAdd(machine, key => new MachineAvailableChecker(machine, this._onCheck, this._onUnAvailable));
				}
			}
		}

		/// <summary>
		/// 清空内部检测器
		/// </summary>
		public void Clear()
		{
			foreach (var checker in this._checkers)
			{
				checker.Value.Complete();
			}
			_cache.KeyDelete(MACHINE_CHECKER);
			_checkers.Clear();
		}
	}
}
