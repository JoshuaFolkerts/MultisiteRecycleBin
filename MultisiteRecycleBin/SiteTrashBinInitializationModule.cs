using EPiServer;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.DataAccess;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System.Linq;

namespace MultisiteRecycleBin
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class SiteTrashBinInitializationModule : IInitializableModule
    {
        private bool _isInitialized;

        private const string siteBinUrlSegment = "__sitetrashbin";

        public void Initialize(InitializationEngine context)
        {
            if (_isInitialized)
            {
                return;
            }

            var locator = context.Locate.Advanced;
            var contentEvents = context.Locate.ContentEvents();
            var contentRepository = context.Locate.ContentRepository();
            var siteDefinitionRepository = locator.GetInstance<ISiteDefinitionRepository>();

            var events = locator.GetInstance<IContentEvents>();
            events.MovedContent += DeleteEvent_MovedContent;
            this.CreateSiteBinFolders(siteDefinitionRepository, contentRepository);

            _isInitialized = true;
        }

        private void DeleteEvent_MovedContent(object sender, EPiServer.ContentEventArgs e)
        {
            if (e.TargetLink.CompareToIgnoreWorkID(ContentReference.WasteBasket) && !(e.Content is ITrashBin))
            {
                var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

                var siteTrashBin = contentRepository.GetChildren<PageData>(SiteDefinition.Current.StartPage)
                    .OfType<ITrashBin>()
                    .FirstOrDefault();

                if (siteTrashBin != null && !ContentReference.IsNullOrEmpty(siteTrashBin.ContentLink))
                    contentRepository.Move(e.ContentLink, siteTrashBin.ContentLink);
            }
        }

        public void CreateSiteBinFolders(ISiteDefinitionRepository siteDefinitionRepository, IContentRepository contentRepository)
        {
            foreach (var siteDefinition in siteDefinitionRepository.List())
            {
                var siteBin = contentRepository.GetBySegment(siteDefinition.StartPage, "__sitetrashbin", LanguageSelector.AutoDetect());

                if (siteBin == null)
                {
                    var trashBin = contentRepository.GetDefault<SiteTrashBinPage>(siteDefinition.StartPage);
                    trashBin.Name = "Waste Basket";
                    trashBin.URLSegment = siteBinUrlSegment;
                    contentRepository.Save(trashBin, SaveAction.Publish, AccessLevel.NoAccess);
                }
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.MovedContent -= DeleteEvent_MovedContent;
        }
    }
}