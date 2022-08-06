using System.Threading.Tasks;
using Framework.Asynchronous;

public interface IBuildTask
{
    string Run(BuildContext context);
}