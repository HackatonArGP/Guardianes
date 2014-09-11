using System.Collections.Generic;
using Earthwatchers.Models;

namespace Earthwatchers.Data
{
    public interface IPopupMessageRepository
    {
        List<PopupMessage> GetMessage();
        List<PopupMessage> GetAllMessages();
        PopupMessage Insert(PopupMessage message);
        void Delete(int id);
    }
}
