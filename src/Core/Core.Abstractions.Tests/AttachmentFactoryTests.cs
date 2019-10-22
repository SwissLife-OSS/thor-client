using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Thor.Core.Abstractions.Tests
{
    public class AttachmentFactoryTests
    {
        #region Create Exception

        [Fact(DisplayName = "Create: Should not throw an argument exception for empty correlationId")]
        public void Create_Exception_CorrelationIdNull()
        {
            // arrange
            ExceptionAttachment attachment = null;
            AttachmentId correlationId = AttachmentId.Empty;
            string payloadName = "Name-773";
            Exception payloadValue = new ArgumentNullException("Value-773");

            // act
            Action verify = () => attachment = AttachmentFactory
                .Create(correlationId, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
            Assert.Null(attachment);
        }

        [Fact(DisplayName = "Create: Should not throw an argument null exception for payloadName")]
        public void Create_Exception_PayloadNameNull()
        {
            // arrange
            ExceptionAttachment attachment = null;
            AttachmentId correlationId = AttachmentId.NewId();
            string payloadName = null;
            Exception payloadValue = new ArgumentNullException("Value-426");

            // act
            Action verify = () => attachment = AttachmentFactory
                .Create(correlationId, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
            Assert.Null(attachment);
        }

        [Fact(DisplayName = "Create: Should not throw an argument null exception for payloadValue")]
        public void Dispatch_Exception_PayloadValueNull()
        {
            // arrange
            ExceptionAttachment attachment = null;
            AttachmentId correlationId = AttachmentId.NewId();
            string payloadName = "Name-567";
            Exception payloadValue = null;

            // act
            Action verify = () => attachment = AttachmentFactory
                .Create(correlationId, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
            Assert.Null(attachment);
        }

        [Fact(DisplayName = "Create: Should create an attachment")]
        public void Dispatch_Exception_Success()
        {
            // arrange
            ExceptionAttachment attachment = null;
            AttachmentId correlationId = AttachmentId.NewId();
            string payloadName = "Name-848";
            Exception payloadValue = new ArgumentNullException("Value-848");

            // act
            Action verify = () => attachment = AttachmentFactory
                .Create(correlationId, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
            Assert.NotNull(attachment);
            Assert.IsType<ExceptionAttachment>(attachment);
        }

        #endregion

        #region Create Object

        [Fact(DisplayName = "Create: Should not throw an argument exception for empty correlationId")]
        public void Create_Object_CorrelationIdNull()
        {
            // arrange
            ObjectAttachment attachment = null;
            AttachmentId correlationId = AttachmentId.Empty;
            string payloadName = "Name-627";
            object payloadValue = "Value-627";

            // act
            Action verify = () => attachment = AttachmentFactory
                .Create(correlationId, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
            Assert.Null(attachment);
        }

        [Fact(DisplayName = "Create: Should not throw an argument null exception for payloadName")]
        public void Create_Object_PayloadNameNull()
        {
            // arrange
            ObjectAttachment attachment = null;
            AttachmentId correlationId = AttachmentId.NewId();
            string payloadName = null;
            object payloadValue = "Value-119";

            // act
            Action verify = () => attachment = AttachmentFactory
                .Create(correlationId, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
            Assert.Null(attachment);
        }

        [Fact(DisplayName = "Create: Should not throw an argument null exception for payloadValue")]
        public void Dispatch_Object_PayloadValueNull()
        {
            // arrange
            ObjectAttachment attachment = null;
            AttachmentId correlationId = AttachmentId.NewId();
            string payloadName = "Name-666";
            object payloadValue = null;

            // act
            Action verify = () => attachment = AttachmentFactory
                .Create(correlationId, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
            Assert.Null(attachment);
        }

        [Fact(DisplayName = "Create: Should create an attachment")]
        public void Dispatch_Object_Success()
        {
            // arrange
            ObjectAttachment attachment = null;
            AttachmentId correlationId = AttachmentId.NewId();
            string payloadName = "Name-123";
            object payloadValue = "Value-123";

            // act
            Action verify = () => attachment = AttachmentFactory
                .Create(correlationId, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
            Assert.NotNull(attachment);
            Assert.IsType<ObjectAttachment>(attachment);
        }

        #endregion

        #region Create Stream

        [Fact(DisplayName = "Create: Should not throw an argument exception for empty correlationId")]
        public async Task Create_Stream_CorrelationIdNull()
        {
            using (MemoryStream payloadValue = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(payloadValue))
            {
                // arrange
                ObjectAttachment attachment = null;
                AttachmentId correlationId = AttachmentId.Empty;
                string payloadName = "Name-555";

                await writer.WriteAsync("Value-555");
                await writer.FlushAsync();

                // act
                Func<Task> verify = async () => attachment = await AttachmentFactory
                    .CreateAsync<ObjectAttachment>(correlationId, payloadName, payloadValue);

                // assert
                Assert.Null(await Record.ExceptionAsync(verify));
                Assert.Null(attachment);
            }
        }

        [Fact(DisplayName = "Create: Should not throw an argument null exception for payloadName")]
        public async Task Create_Stream_PayloadNameNull()
        {
            using (MemoryStream payloadValue = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(payloadValue))
            {
                // arrange
                ObjectAttachment attachment = null;
                AttachmentId correlationId = AttachmentId.NewId();
                string payloadName = null;

                await writer.WriteAsync("Value-444");
                await writer.FlushAsync();

                // act
                Func<Task> verify = async () => attachment = await AttachmentFactory
                    .CreateAsync<ObjectAttachment>(correlationId, payloadName, payloadValue);

                // assert
                Assert.Null(await Record.ExceptionAsync(verify));
                Assert.Null(attachment);
            }
        }

        [Fact(DisplayName = "Create: Should not throw an argument null exception for payloadValue")]
        public async Task Dispatch_Stream_PayloadValueNull()
        {
            // arrange
            ObjectAttachment attachment = null;
            AttachmentId correlationId = AttachmentId.NewId();
            string payloadName = "Name-881";
            Stream payloadValue = null;

            // act
            Func<Task> verify = async () => attachment = await AttachmentFactory
                .CreateAsync<ObjectAttachment>(correlationId, payloadName, payloadValue);

            // assert
            Assert.Null(await Record.ExceptionAsync(verify));
            Assert.Null(attachment);
        }

        [Fact(DisplayName = "Create: Should create an attachment")]
        public async Task Dispatch_Stream_Success()
        {
            using (MemoryStream payloadValue = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(payloadValue))
            {
                // arrange
                ObjectAttachment attachment = null;
                AttachmentId correlationId = AttachmentId.NewId();
                string payloadName = "Name-919";

                await writer.WriteAsync("Value-919");
                await writer.FlushAsync();

                // act
                Func<Task> verify = async () => attachment = await AttachmentFactory
                    .CreateAsync<ObjectAttachment>(correlationId, payloadName, payloadValue);

                // assert
                Assert.Null(await Record.ExceptionAsync(verify));
                Assert.NotNull(attachment);
                Assert.IsType<ObjectAttachment>(attachment);
                Assert.Equal("\"Value-919\"", Encoding.UTF8.GetString(attachment.Value));
            }
        }

        #endregion
    }
}