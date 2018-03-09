using System.Xml;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Sites;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
using Sitecore.Xml;
using Sitecore.XA.Foundation.Multisite;
using Microsoft.Extensions.DependencyInjection;

namespace Sitecore.Support.XA.Foundation.Multisite.LinkManagers
{
  public class LinkItem : Sitecore.XA.Foundation.Multisite.LinkManagers.LinkItem
  {
    private string Url { get; set; }

    #region 11594
    private string Anchor { get; set; }
    #endregion

    private string QueryString { get; set; }

    public LinkItem(string xml) : base(xml)
    {
      if (string.IsNullOrEmpty(xml))
      {
        return;
      }

      XmlNode node = XmlUtil.GetXmlNode(xml);
      if (node == null)
      {
        return;
      }

      if (node.Name == "link" && node.Attributes != null)
      {
        XmlAttribute urlAttr = node.Attributes["url"];
        if (urlAttr != null)
        {
          Url = urlAttr.Value;
        }
        #region 11594
        XmlAttribute typeAttr = node.Attributes["anchor"];
        if (typeAttr != null)
        {
          Anchor = "#" + typeAttr.Value;
        }
        #endregion
        #region 11146
        XmlAttribute queryStrAttr = node.Attributes["querystring"];
        if (queryStrAttr != null)
        {

          QueryString = queryStrAttr.Value;
        }
        #endregion
      }
    }
    public override string TargetUrl
    {
      get
      {
        if (TargetItem != null)
        {
          if (IsMediaLink || TargetItem.Paths.IsMediaItem)
          {
            return MergeQueryStrings(((MediaItem)TargetItem).GetMediaUrl());
          }

          if (IsInternal)
          {
            var targetSiteInfo = Sitecore.DependencyInjection.ServiceLocator.ServiceProvider.GetService<ISiteInfoResolver>().GetSiteInfo(TargetItem);
            var urlOptions = (UrlOptions)UrlOptions.DefaultOptions.Clone();
            if (targetSiteInfo != null)
            {
              urlOptions.Site = new SiteContext(targetSiteInfo);

              urlOptions.LanguageEmbedding = LanguageEmbedding.Never;
            }

            return MergeQueryStrings(LinkManager.GetItemUrl(TargetItem, urlOptions));
          }
        }

        return Url;
      }
    }

    #region 11594
    protected virtual string MergeQueryStrings(string link)
    {
      if (link.Contains("?"))
      {
        return link + "&" + QueryString + Anchor;
      }
      return link + "?" + QueryString + Anchor;
    }
    #endregion
  }
}