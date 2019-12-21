using System;
using IonDotnet.Internals;

namespace IonDotnet.Tree.Impl
{
    internal sealed class IonDecimal : IonValue, IIonDecimal
    {
        private BigDecimal _val;

        public IonDecimal(double doubleValue) : this(Convert.ToDecimal(doubleValue))
        {
        }

        public IonDecimal(decimal value) : this(new BigDecimal(value))
        {
        }

        public IonDecimal(BigDecimal bigDecimal) : base(false)
        {
            _val = bigDecimal;
        }

        private IonDecimal(bool isNull) : base(isNull)
        {
        }

        public static IonDecimal NewNull() => new IonDecimal(true);

        public override bool IsEquivalentTo(IIonValue other)
        {
            if (!base.IsEquivalentTo(other))
                return false;

            var otherDec = (IonDecimal)other;

            if (NullFlagOn())
                return otherDec.IsNull;
            if (other.IsNull)
                return false;

            if (BigDecimalValue.IsNegativeZero ^ otherDec.BigDecimalValue.IsNegativeZero)
                return false;

            if (otherDec.BigDecimalValue.Scale > 0 || BigDecimalValue.Scale > 0)
            {
                //precision matters, make sure that this has the same precision
                return BigDecimalValue.Scale == otherDec.BigDecimalValue.Scale
                       && BigDecimalValue.IntVal == otherDec.BigDecimalValue.IntVal;
            }

            //this only compares values
            return BigDecimalValue == otherDec.BigDecimalValue;
        }

        internal override void WriteBodyTo(IPrivateWriter writer)
        {
            if (NullFlagOn())
            {
                writer.WriteNull(IonType.Decimal);
                return;
            }

            writer.WriteDecimal(BigDecimalValue);
        }

        public override decimal DecimalValue
        {
            get
            {
                ThrowIfNull();
                return _val.ToDecimal();
            }
            set
            {
                ThrowIfLocked();
                _val = new BigDecimal(value);
            }
        }

        public override BigDecimal BigDecimalValue
        {
            get
            {
                ThrowIfNull();
                return _val;
            }
            set
            {
                ThrowIfLocked();
                _val = value;
            }
        }

        public override IonType Type => IonType.Decimal;
    }
}
