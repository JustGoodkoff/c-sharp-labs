using EveryoneToTheHackathon.Host;

namespace EveryoneToTheHackathon.DataContracts;

public class Hackathon : IHackathon
{
    private readonly List<Employee> _teamLeads;
    private int _teamLeadsNumber;
    private readonly List<Employee> _juniors;
    private int _juniorsNumber;
    private readonly HRManager _hrManager;
    private readonly HRDirector _hrDirector;
    
    private List<Wishlist>? _juniorsWishlists;
    private List<Wishlist>? _teamLeadsWishlists;
    private List<Team>? _teams;
    
    private double _meanSatisfactionIndex = -1;
    
    public double MeanSatisfactionIndex => _meanSatisfactionIndex;

    public Hackathon(
        HRManager hrManager, HRDirector hrDirector)
    {
        _teamLeads =  CsvParser.ParseCsvFileWithEmployees("Resources/Teamleads20.csv").ToList();
        _juniors = CsvParser.ParseCsvFileWithEmployees("Resources/Juniors20.csv").ToList();
        _juniorsNumber = 20;
        _teamLeadsNumber = 20;
        _hrManager = hrManager;
        _hrDirector = hrDirector;
    }

 public Hackathon( IEnumerable<Employee> teamLeads, int teamLeadsNumber, 
        IEnumerable<Employee> juniors, int juniorsNumber, 
        HRManager hrManager, HRDirector hrDirector)
    {
        _juniors = juniors.ToList();
        _juniorsNumber = juniorsNumber;
        _teamLeads = teamLeads.ToList(); 
        _teamLeadsNumber = teamLeadsNumber;
        _hrManager = hrManager;
        _hrDirector = hrDirector;
    }



    public void HoldEvent()
    {
        _teamLeadsWishlists = _teamLeads.Select(teamlead => teamlead.MakeWishlist(_juniors)).ToList();
        _juniorsWishlists = _juniors.Select(junior => junior.MakeWishlist(_teamLeads)).ToList();

         _teams = _hrManager.BuildTeams(_teamLeads, _juniors, _teamLeadsWishlists, _juniorsWishlists).ToList();
         _meanSatisfactionIndex = _hrDirector.CalculateMeanSatisfactionIndex(_teamLeadsWishlists, _juniorsWishlists, _teams);
    }

    public void HoldEvent(IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists)
    {
        _teamLeadsWishlists = (List<Wishlist>)teamLeadsWishlists;
        _juniorsWishlists = (List<Wishlist>)juniorsWishlists;

         _teams = _hrManager.BuildTeams(_teamLeads, _juniors, _teamLeadsWishlists, _juniorsWishlists).ToList();
         _meanSatisfactionIndex = _hrDirector.CalculateMeanSatisfactionIndex(_teamLeadsWishlists, _juniorsWishlists, _teams);

    }

}