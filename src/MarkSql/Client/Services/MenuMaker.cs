using System;
using Humanizer;
using MarkSql.Shared;

namespace MarkSql.Client.Services
{

    public interface IMenuMaker
    {
        public List<MasMenu> BuildMenu(MarkModel model, string tag);
    }

    public class MenuMaker : IMenuMaker
    {
        public MenuMaker()
        {
        }
        public List<MasMenu> BuildMenu(MarkModel model, string tag)
        {
            var menus = new List<MasMenu>();
            foreach (var proc in model.procs)
            {
                var uri = "parameterform/" + proc.name + "?";
                foreach (var par in proc.parameters)
                {
                    uri += par.name + "=";
                    uri += "&";
                }
                uri = uri.Substring(0, uri.Length - 1);
                var menuText = proc.name.Substring(proc.name.IndexOf('_') + 1 );
                menuText = menuText.Humanize();
                menus.Add( new MasMenu { Text = menuText, Uri = uri });
            }
            return menus;
        }
    }
}

