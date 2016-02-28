using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

/*
 * This code is derived from the code found in the article Programmatically adding attachments 
 * to emails in C# and VB.NET that can be found at http://www.codeproject.com/KB/IP/SendFileToNET.aspx.
 * Additional information about MAPI can be found on MSDN: http://msdn.microsoft.com/en-us/library/ms529058%28EXCHG.10%29.aspx.
 */

namespace BizArk.Core.Email
{
    internal class MAPI32
    {
        public const int cMapiLogonUI = 0x00000001;
        public const int cMapiDialog = 0x00000008;
        private static readonly string[] cErrors = new string[] {
            "OK [0]", 
            "User abort [1]", 
            "General MAPI failure [2]", 
            "MAPI login failure [3]", 
            "Disk full [4]", 
            "Insufficient memory [5]", 
            "Access denied [6]", 
            "-unknown- [7]", 
            "Too many sessions [8]", 
            "Too many files were specified [9]", 
            "Too many recipients were specified [10]", 
            "A specified attachment was not found [11]",
            "Attachment open failure [12]", 
            "Attachment write failure [13]", 
            "Unknown recipient [14]", 
            "Bad recipient type [15]", 
            "No messages [16]", 
            "Invalid message [17]", 
            "Text too large [18]", 
            "Invalid session [19]", 
            "Type not supported [20]", 
            "A recipient was specified ambiguously [21]", 
            "Message in use [22]", 
            "Network failure [23]",
            "Invalid edit fields [24]", 
            "Invalid recipients [25]", 
            "Not supported [26]" 
        };

        public static SendStatus SendMail(Message msg, bool showDialog)
        {
            var mapiMsg = new MapiMessage();
            mapiMsg.Subject = msg.Subject;
            mapiMsg.NoteText = msg.Body;

            var recipients = new List<MapiRecipDesc>();
            recipients.AddRange(GetRecipients(msg.To));
            recipients.AddRange(GetRecipients(msg.CC));
            recipients.AddRange(GetRecipients(msg.BCC));
            mapiMsg.Recipients = AllocMemory<MapiRecipDesc>(recipients.ToArray(), out mapiMsg.RecipientCount);

            var attachments = GetAttachments(msg.Attachments.ToArray());
            mapiMsg.Files = AllocMemory<MapiFileDesc>(attachments, out mapiMsg.FileCount);

            int flags = showDialog ? MAPI32.cMapiLogonUI | MAPI32.cMapiDialog : MAPI32.cMapiLogonUI;
            int retCode = MAPISendMail(IntPtr.Zero, IntPtr.Zero, mapiMsg, flags, 0);

            FreeMemory(mapiMsg.Recipients, typeof(MapiRecipDesc), mapiMsg.RecipientCount);
            FreeMemory(mapiMsg.Files, typeof(MapiFileDesc), mapiMsg.FileCount);

            if (retCode == 0)
                return SendStatus.Ok;
            else if (retCode == 1)
                return SendStatus.Cancel;
            else
                throw new Mapi32Exception(cErrors[retCode], retCode);
        }

        private static MapiFileDesc[] GetAttachments(string[] attachments)
        {
            var mapiAttachments = new List<MapiFileDesc>();
            foreach (string attachment in attachments)
            {
                MapiFileDesc mapiFileDesc = new MapiFileDesc();
                mapiFileDesc.Position = -1;
                mapiFileDesc.Name = Path.GetFileName(attachment);
                mapiFileDesc.Path = attachment;
                mapiAttachments.Add(mapiFileDesc);
            }

            return mapiAttachments.ToArray();
        }

        private static IntPtr AllocMemory<T>(T[] values, out int count)
        {
            count = values.Length;
            if (count == 0)
                return IntPtr.Zero;

            int size = Marshal.SizeOf(typeof(T));
            IntPtr intPtr = Marshal.AllocHGlobal(count * size);
            int ptr = (int)intPtr;
            foreach (T value in values)
            {
                Marshal.StructureToPtr(value, (IntPtr)ptr, false);
                ptr += size;
            }

            return intPtr;
        }

        private static void FreeMemory(IntPtr ptr, Type type, int count)
        {
            if (ptr == IntPtr.Zero) return;

            int size = Marshal.SizeOf(type);
            int pos = (int)ptr;
            for (int i = 0; i < count; i++)
            {
                Marshal.DestroyStructure((IntPtr)pos, type);
                pos += size;
            }
            Marshal.FreeHGlobal(ptr);
        }

        private static MapiRecipDesc[] GetRecipients(EmailAddressList addresses)
        {
            var recipients = new List<MapiRecipDesc>();

            foreach (EmailAddress addr in addresses)
            {
                var mapiDesc = new MapiRecipDesc();
                if (string.IsNullOrEmpty(addr.Name))
                {
                    // If the recipients name was not specified, we must
                    // set the name to the email address or MAPI32 will
                    // return an error.
                    mapiDesc.Name = addr.Address;
                }
                else
                {
                    mapiDesc.Name = addr.Name;
                    mapiDesc.Address = addr.Address;
                }
                mapiDesc.RecipientClass = (uint)addresses.AddressType;
                recipients.Add(mapiDesc);
            }

            return recipients.ToArray();
        }

        [DllImport("MAPI32.DLL", CharSet = CharSet.Unicode)]
        static extern int MAPISendMail(IntPtr sess, IntPtr hwnd, MapiMessage message, int flg, int rsv);
    }

    /// <summary>
    /// The status from a MAPI send request.
    /// </summary>
    public enum SendStatus
    {
        /// <summary>The email was sent.</summary>
        Ok,
        /// <summary>The email was cancelled by the user.</summary>
        Cancel
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal class MapiMessage
    {
        /// <summary>
        /// Reserved; must be zero.
        /// </summary>
        public int Reserved;
        /// <summary>
        /// Pointer to the text string describing the message subject, typically limited to 256 characters or less. If this member is empty or NULL, the user has not entered subject text.
        /// </summary>
        public string Subject;
        /// <summary>
        /// Pointer to a string containing the message text. If this member is empty or NULL, there is no message text.
        /// </summary>
        public string NoteText;
        /// <summary>
        /// Pointer to a string indicating a non-IPM type of message. Client applications can select message types for their non-IPM messages. Clients that only support IPM messages can ignore the lpszMessageType member when reading messages and set it to empty or NULL when sending messages.
        /// </summary>
        public string MessageType;
        /// <summary>
        /// Pointer to a string indicating the date when the message was received. The format is YYYY/MM/DD HH:MM, using a 24-hour clock.
        /// </summary>
        public string DateReceived;
        /// <summary>
        /// Pointer to a string identifying the conversation thread to which the message beints. Some messaging systems can ignore and not return this member.
        /// </summary>
        public string ConversationID;
        /// <summary>
        /// Bitmask of message status flags
        /// </summary>
        public int Flags;
        /// <summary>
        /// Pointer to a MapiRecipDesc structure containing information about the sender of the message.
        /// </summary>
        public IntPtr Originator;
        /// <summary>
        /// The number of message recipient structures in the array pointed to by the lpRecips member. A value of zero indicates no recipients are included.
        /// </summary>
        public int RecipientCount;
        /// <summary>
        /// Pointer to an array of MapiRecipDesc structures, each containing information about a message recipient.
        /// </summary>
        public IntPtr Recipients;
        /// <summary>
        /// The number of structures describing file attachments in the array pointed to by the lpFiles member. A value of zero indicates no file attachments are included.
        /// </summary>
        public int FileCount;
        /// <summary>
        /// Pointer to an array of MapiFileDesc structures, each containing information about a file attachment.
        /// </summary>
        public IntPtr Files;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal class MapiFileDesc
    {
        public int Reserved;
        public int Flags;
        public int Position;
        public string Path;
        public string Name;
        public IntPtr Type;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal class MapiRecipDesc
    {
        public uint Reserved;
        public uint RecipientClass;
        public string Name;
        public string Address;
        public int EIDSize;
        public IntPtr EntryID;
    }

    /// <summary>
    /// Exceptions thrown from MAPI32.dll.
    /// </summary>
    public class Mapi32Exception
        : ApplicationException
    {
        /// <summary>
        /// Creates an instance of Mapi32Exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errCode"></param>
        public Mapi32Exception(string message, int errCode)
            : base(message)
        {
            ErrorCode = errCode;
        }

        /// <summary>
        /// Gets the error code as returned by MAPISendMail.
        /// </summary>
        public int ErrorCode { get; private set; }
    }
}
