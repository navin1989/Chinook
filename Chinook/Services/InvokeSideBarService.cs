using static Chinook.Services.InvokeSideBarService;

namespace Chinook.Services
{
    //"InvokeSideBarService" service will invoke the NavBar to be rerendered.
    //So that Navigation will load the playlists without page reloading. 
    public class InvokeSideBarService
    {
        public IndexEventArgs currentEventArgs = new IndexEventArgs();
        public event EventHandler<IndexEventArgs> changeEvent;
        public class IndexEventArgs : EventArgs { }
        public void Invoke()
        {
            if (currentEventArgs != null)
            {
                changeEvent.Invoke(this, currentEventArgs);
            }
        }
    }
}
