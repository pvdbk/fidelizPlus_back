using System;
using System.Linq;
using System.Collections.Generic;

namespace fidelizPlus_back.Payment
{
	public class Monitor
	{
		private Dictionary<string, IEnumerable<Action>> events { get; set; }

		public Monitor()
		{
			events = new Dictionary<string, IEnumerable<Action>>();
		}

		public void Subscribe(string e, Action action)
		{
			if(!events.ContainsKey(e))
			{
				events.Add(e, Enumerable.Empty<Action>());
			}
			events[e] = events[e].Append(action);
		}

		public void UnSubscribe(string e, Action action)
		{
			if (events.ContainsKey(e))
			{
				events[e] = events[e].Where(a => a != action);
				if(events[e].Count() == 0)
				{
					events.Remove(e);
				}
			}
			if (!events.ContainsKey(e))
			{
			}
		}

		public void Signal(string e)
		{
			if (events.ContainsKey(e))
			{
				events[e].ForEach(a => a());
			}
		}
	}
}
