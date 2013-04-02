namespace Orc.DataStructures.AList.Implementations
{
    using System.Diagnostics;

    class ArrayOf4<T>
	{
		T t0, t1, t2, t3;
		
		T A { get { return this.t0; } set { this.t0 = value; } }
		T B { get { return this.t1; } set { this.t1 = value; } }
		T C { get { return this.t2; } set { this.t2 = value; } }
		T D { get { return this.t3; } set { this.t3 = value; } }

		public T this[int index]
		{
			get {
				Debug.Assert((uint)index < (uint)4);
				if (index < 2)
					return index > 0 ? this.t1 : this.t0;
				else
					return index == 2 ? this.t2 : this.t3;
			}
			set {
				Debug.Assert((uint)index < (uint)4);
				if (index < 2) {
					if (index == 0)
						this.t0 = value;
					else
						this.t1 = value;
				} else {
					if (index == 2)
						this.t2 = value;
					else
						this.t3 = value;
				}
			}
		}
		public T Insert(int index, T item)
		{
			T popped = this.t3;
			this.t3 = this.t2;
			if (index < 2) {
				this.t2 = this.t1;
				if (index == 0)
					this.t0 = item;
				else
					this.t1 = item;
				return popped;
			} else {
				if (index == 2)
					this.t2 = item;
				else
					this.t3 = item;
				return popped;
			}
		}
		public void RemoveAt(int index, T newFourth)
		{
			if (index < 2)
			{
				if (index == 0)
					this.t0 = this.t1;
				this.t1 = this.t2;
			}
			if (index == 2)
				this.t2 = this.t3;
			this.t3 = newFourth;
		}
		public T First
		{
			get { return this.t0; }
			set { this.t0 = value; }
		}
	}
}
