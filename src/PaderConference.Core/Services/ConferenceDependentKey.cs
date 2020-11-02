namespace PaderConference.Core.Services
{
    public class ConferenceDependentKey
    {
        private readonly string _postFix;

        public ConferenceDependentKey(string postFix)
        {
            _postFix = postFix;
        }

        public string GetName(string conferenceId)
        {
            return conferenceId + _postFix;
        }

        public bool Match(string s)
        {
            return s.EndsWith(_postFix);
        }
    }
}
