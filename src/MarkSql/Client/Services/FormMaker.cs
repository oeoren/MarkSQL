using MarkSql.Shared;

namespace MarkSql.Client.Services
{
    public interface IFormMaker
    {
        public MasForm? BuildForm(MarkModel model, string procName);
    }

    public class FormMaker : IFormMaker
    {
        public FormMaker()
        {
        }

        /// <summary>
        /// Build a MasForm object for a procedure
        /// </summary>
        /// <param name="model"></param>
        /// <param name="procName"></param>
        /// <returns></returns>
        public MasForm? BuildForm(MarkModel? model, string procName)
        {
            var procInfo = model.procs.FirstOrDefault(p => (p.name == procName));
            if (procInfo == null)
                return null;
            MasForm form = new MasForm { Procname = procInfo.name };
            foreach (var par in procInfo.parameters)
            {
                switch (par.sqlType)
                {
                    case "date":
                    case "datetime":
                        form.Fields.Add(
                            new MasField
                            {
                                FormValue = "",
                                MasType = "date",
                                Name = par.name,
                                Value = ""
                            });
                    break;
                    default:
                        form.Fields.Add(
                            new MasField
                            {
                                FormValue = "",
                                MasType = "string",
                                Name = par.name,
                                Value = ""
                            }
                            );
                            break;
                 }
            }
            return form;
        }
    }
}
