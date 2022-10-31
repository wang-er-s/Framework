using System.Threading.Tasks;

public interface IBuildTask
{
    string Run(BuildContext context);
}