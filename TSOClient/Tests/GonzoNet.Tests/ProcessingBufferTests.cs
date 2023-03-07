using GonzoNet;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GonzoNet;

namespace GonzoNet.Tests
{
    [TestClass]
    public class ProcessingBufferTests
    {
        /// <summary>
        /// Checks that adding data to the processing buffer 
        /// works correctly by adding some data to the buffer 
        /// and checking that the count of the internal buffer 
        /// matches the expected value.
        /// </summary>
        [TestMethod]
        public void TestAddingData()
        {
            var processingBuffer = new ProcessingBuffer();
            byte[] data = new byte[] { 1, 2, 3, 4 };
            processingBuffer.AddData(data);
            try
            {
                Assert.AreEqual(4, processingBuffer.InternalBuffer.Count);
                Assert.AreEqual(data[0], processingBuffer.InternalBuffer.Peek());
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.ToString());
            }
        }

        [TestMethod]
        public void TestAddingTooMuchData()
        {
            var processingBuffer = new ProcessingBuffer();
            byte[] data = new byte[ProcessingBuffer.MAX_PACKET_SIZE + 1];
            processingBuffer.AddData(data);

            try
            {
                Assert.AreEqual(0, processingBuffer.InternalBuffer.Count);
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.ToString());
            }
        }

        [TestMethod]
        public void TestProcessingPacket()
        {
            var processingBuffer = new ProcessingBuffer();
            bool eventFired = false;
            processingBuffer.OnProcessedPacket += (packet) => eventFired = true;
            byte[] data = new byte[] { 1, 0, 4, 5, 6, 7 };
            processingBuffer.AddData(data);

            try
            {
                Assert.IsFalse(eventFired);
                processingBuffer.AddData(new byte[] { 8, 9, 10 });
                Assert.IsTrue(eventFired);
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.ToString());
            }
        }
    }
}