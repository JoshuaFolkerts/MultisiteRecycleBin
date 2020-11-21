using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Web;
using System.Linq;

namespace MultisiteRecycleBin
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class TrashBinEventInitialization : IInitializableModule
    {
        private IContentRepository contentRepository;

        public void Initialize(InitializationEngine context)
        {
            var events = context.Locate.Advanced.GetInstance<IContentEvents>();
            events.MovedContent += DeleteEvent_MovedContent;
            this.contentRepository = context.Locate.ContentRepository();
        }

        private void DeleteEvent_MovedContent(object sender, EPiServer.ContentEventArgs e)
        {
            if (e.TargetLink.CompareToIgnoreWorkID(ContentReference.WasteBasket))
            {
                var siteTrashBin = this.contentRepository.GetChildren<PageData>(SiteDefinition.Current.StartPage)
                    .OfType<ITrashBin>()
                    .FirstOrDefault();
                if (siteTrashBin != null && !ContentReference.IsNullOrEmpty(siteTrashBin.ContentLink))
                {
                    this.contentRepository.Move(e.ContentLink, siteTrashBin.ContentLink);
                }
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            var events = context.Locate.Advanced.GetInstance<IContentEvents>();
            events.MovedContent -= DeleteEvent_MovedContent;
        }
    }
}