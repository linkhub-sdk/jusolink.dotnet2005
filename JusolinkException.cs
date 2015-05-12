using System;

namespace Jusolink
{
    public class JusolinkException : Exception
    {
        public JusolinkException()
            : base()
        {
        }
        public JusolinkException(long code, String Message)
            : base(Message)
        {
            this._code = code;
        }

        private long _code;

        public long code
        {
            get { return _code; }
        }
        public JusolinkException(Linkhub.LinkhubException le)
            : base(le.Message, le)
        {
            this._code = le.code;
        }

    }
}
