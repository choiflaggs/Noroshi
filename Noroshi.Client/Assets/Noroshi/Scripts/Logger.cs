namespace Noroshi
{
    public class Logger
    {
        public Logger()
        {
        }
        public void Debug(object content)
        {
            UnityEngine.Debug.Log(content);
        }
        public void Error(object content)
        {
            UnityEngine.Debug.LogError(content);
        }
    }
}