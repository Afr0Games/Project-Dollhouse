using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sound
{
    /// <summary>
    /// C# implementation of a Coroutine, from: http://www.gamedev.net/blog/1612/entry-2259253-coroutines-building-a-framework-in-c/
    /// </summary>
    public abstract class Coroutine
    {
        private IEnumerator<object> _enumerator;
        private bool _do_sub = false;
        private Coroutine _sub_coroutine;
        public bool is_complete = false;
        public bool can_move_next = true;
        private object _sub_input = null;

        public Coroutine()
        {
            this._enumerator = this.process().GetEnumerator();
        }

        public object YieldFrom(Coroutine coroutine, object sub_input = null)
        {
            this._do_sub = true;
            this._sub_coroutine = coroutine;
            this._sub_input = sub_input;
            return this._sub_coroutine.next();
        }

        public object YieldComplete(object return_value = null)
        {
            this.is_complete = true;
            return return_value;
        }

        public object next(object in_value = null)
        {
            if (this._do_sub)
            {
                if (this._sub_coroutine.can_move_next && !this._sub_coroutine.is_complete)
                    return this._sub_coroutine.next(this._sub_input);
                else
                {
                    this._do_sub = false;
                    this._sub_input = in_value;
                    this.can_move_next = this._enumerator.MoveNext();
                    return this._enumerator.Current;
                }
            }
            else
            {
                this._sub_input = in_value;
                this.can_move_next = this._enumerator.MoveNext();
                return this._enumerator.Current;
            }
        }

        public abstract IEnumerable<object> process();
    }
}
