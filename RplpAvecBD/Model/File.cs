
namespace RplpAvecBD.Model
{
    public class File
    {
        public string name { get; set; } //le nome du fichier
        public string code { get; set; } //le contenu du fichier
        public string extension { get; set; } //l'extansion du fichier
        public int submission { get; set; } //le numero de submission pour Upload du fichier
        public string path { get; set; } //la route ver le fichier

        public File(string p_name, string p_extension, string p_code, int p_submission, string p_path)
        {
            this.path = p_path;
            this.name = p_name;
            this.code = p_code;
            this.extension = p_extension;
            this.submission = p_submission;
        }
    }
}
