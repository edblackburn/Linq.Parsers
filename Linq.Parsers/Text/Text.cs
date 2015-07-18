﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linq.Parsers
{
    public abstract class Text : IEnumerable<char>
    {
        #region Fields

        internal string value;
        private bool isEvaluated;

        #endregion //Fields

        #region Properties

        public abstract int Length { get; }
        public abstract char this[int index] { get; }

        #endregion //Properties

        #region Base Class Overrides

        public override string ToString()
        {
            if (this.isEvaluated)
                return this.value;

            this.value = Evaluate();
            this.isEvaluated = true;
            return this.value;
        }

        #endregion //Base Class Overrides

        #region IEnumerable Members

        public abstract IEnumerator<char> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((Text)this).GetEnumerator();
        }

        #endregion //IEnumerable Members

        #region Methods

        public Text Append(Text tail)
        {
            tail.AssertNotNull();
            if (this.Length == 0)
                return tail;
            if (tail.Length == 0)
                return this;

            var simpleTail = tail as SimpleText;
            if (simpleTail != null)
                return this.AppendSimpleText(simpleTail);

            return this.AppendComplexText((ComplexText)tail);
        }

        public abstract SplitText Split(int index);
        internal abstract bool IsSimpleTextAppendableTo(SimpleText tail);
        internal abstract bool IsComplexTextAppendableTo(ComplexText tail);
        internal abstract Text AppendSimpleText(SimpleText tail);
        internal abstract Text AppendComplexText(ComplexText tail);
        internal abstract string Evaluate();

        #endregion //Methods

        #region Static Members

        #region Fields

        private static Text empty = new SimpleText(string.Empty, 0, 0);

        #endregion //Fields

        #region Properties

        public static Text Empty
        {
            get { return empty; }
        }

        #endregion //Properties

        #region Methods

        #region Create

        public static Text Create(string value)
        {
            value.AssertNotNull();
            return new SimpleText(value, 0, value.Length);
        }

        public static Text Create(char value)
        {
            return new SimpleText(value.ToString(), 0, 1);
        }

        #endregion //Create

        #region Join

        public static Text Join(params Text[] texts)
        {
            return Join((IEnumerable<Text>)texts);
        }

        public static Text Join(IEnumerable<Text> texts)
        {
            texts.AssertNotNull();
            var result = new ComplexText(texts);
            var resultComponents = result.texts;

            if (resultComponents.Count == 0)
                return Text.Empty;
            if (resultComponents.Count == 1)
                return resultComponents[0];

            return result;
        }

        #endregion //Join

        #region IsEmpty

        public static bool IsEmpty(Text text)
        {
            text.AssertNotNull();
            return text.Length == 0;
        }

        #endregion //IsEmpty

        #endregion //Methods

        #endregion //Static Members
    }
}