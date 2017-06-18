using System;
using System.IO;
using System.Threading.Tasks;

namespace Contacts.Interfaces
{
    public interface IPicturePicker
    {
        Task<Stream> GetImageStreamAsync();
    }
}
