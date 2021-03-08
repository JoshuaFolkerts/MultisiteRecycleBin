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

        private IContentRepository contentRepository;

        private ISiteDefinitionRepository siteDefinitionRepository;

        public void Initialize(InitializationEngine context)
        {
            if (_isInitialized)
            {
                return;
            }

            var locator = context.Locate.Advanced;
            var contentEvents = context.Locate.ContentEvents();
            this.contentRepository = context.Locate.ContentRepository();
            this.siteDefinitionRepository = locator.GetInstance<ISiteDefinitionRepository>();

            var events = locator.GetInstance<IContentEvents>();
            events.MovedContent += DeleteEvent_MovedContent;
            this.CreateSiteBinFolders();

            _isInitialized = true;
        }

        private void DeleteEvent_MovedContent(object sender, EPiServer.ContentEventArgs e)
        {
            //if (e.TargetLink.CompareToIgnoreWorkID(ContentReference.WasteBasket) && !(e.Content is ITrashBin))
            //{
            //    var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

            //    var siteTrashBin = contentRepository.GetChildren<PageData>(SiteDefinition.Current.StartPage)
            //        .OfType<ITrashBin>()
            //        .FirstOrDefault();

            //    if (siteTrashBin != null && !ContentReference.IsNullOrEmpty(siteTrashBin.ContentLink))
            //        contentRepository.Move(e.ContentLink, siteTrashBin.ContentLink);
            //}
            if (e.Content != null)
            {
                ITrashBin siteTrashBin = this.contentRepository.GetChildren<IContent>(SiteDefinition.Current.StartPage)
                    .OfType<ITrashBin>()
                    .FirstOrDefault();

                if (e.TargetLink.CompareToIgnoreWorkID(ContentReference.WasteBasket))
                {
                    if (siteTrashBin != null && !ContentReference.IsNullOrEmpty(siteTrashBin?.ContentLink))
                    {
                        var originalParent = ((MoveContentEventArgs)e).OriginalParent;
                        if (!originalParent.CompareToIgnoreWorkID(siteTrashBin?.ContentLink))
                        {
                            this.contentRepository.Move(e.ContentLink, siteTrashBin.ContentLink);
                        }
                    }
                }
            }
        }

        public void CreateSiteBinFolders()
        {
            foreach (var siteDefinition in this.siteDefinitionRepository.List())
            {
                var siteBin = this.contentRepository.GetBySegment(siteDefinition.StartPage, "__sitetrashbin", LanguageSelector.AutoDetect());

                if (siteBin == null)
                {
                    var trashBin = this.contentRepository.GetDefault<SiteTrashBinPage>(siteDefinition.StartPage);
                    trashBin.Name = "Waste Basket";
                    trashBin.URLSegment = siteBinUrlSegment;
                    this.contentRepository.Save(trashBin, SaveAction.Publish, AccessLevel.NoAccess);
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