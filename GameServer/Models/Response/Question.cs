using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class Question
{
    public string id { get; set; }
    public string question { get; set; }
    public string type { get; set ; }
    public List<Answer> answers { get; set; }

}