using System.Text;

namespace Parlo.Docker
{
    /// <summary>
    /// Progressbar in the console.
    /// Based on: https://gist.github.com/DanielSWolf/0ab6a96899cc5377bf54
    /// </summary>
    internal class ConsoleProgressbar
    {
        private const int blockCount = 10;
        private const string animation = @"|/-\";

        private double currentProgress = 0;
        private string currentText = string.Empty;
        private bool disposed = false;
        private int animationIndex = 0;

        public ConsoleProgressbar()
        {
        }

        public void Report(double value)
        {
            if (disposed) return;
            value = Math.Max(0, Math.Min(1, value));

            int progressBlockCount = (int)(value * blockCount);
            int percent = (int)(value * 100);
            string text = string.Format("[{0}{1}] {2,3}% {3}",
                new string('#', progressBlockCount), new string('-', blockCount - progressBlockCount),
                percent,
                animation[animationIndex++ % animation.Length]);

            UpdateText(text);

            Interlocked.Exchange(ref currentProgress, value);
        }

        private void UpdateText(string text)
        {
            int commonPrefixLength = 0;
            int commonLength = Math.Min(currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == currentText[commonPrefixLength])
                commonPrefixLength++;

            StringBuilder outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', currentText.Length - commonPrefixLength);

            outputBuilder.Append(text.Substring(commonPrefixLength));

            int overlapCount = currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }

            Console.Out.Write(outputBuilder.ToString());
            currentText = text;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            UpdateText(string.Empty);
        }
    }
}
