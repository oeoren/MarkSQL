using System;
using Microsoft.AspNetCore.WebUtilities;

namespace MarkSql.Shared
{
	public class FormUtil
	{
		public static MasForm? GetFormAndValues(MasForm form, Uri uri )
        {
            var parameters = QueryHelpers.ParseQuery(uri.Query);
            var items = parameters.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
            if (items != null && form != null)
            {
                foreach (var p in items)
                {
                    foreach (var field in form.Fields)
                        if (field.Name.ToLower() == p.Key.ToLower())
                            field.Value = p.Value;
                }
            }
            return form;

        }
    }
}

