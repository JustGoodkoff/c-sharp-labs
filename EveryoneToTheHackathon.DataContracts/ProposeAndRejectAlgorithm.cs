using System.Diagnostics;

namespace EveryoneToTheHackathon.DataContracts;

public class ProposeAndRejectAlgorithm : ITeamBuildingStrategy
{


    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors, IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        var teamLeadsList = new List<Employee>(teamLeads);
        var juniorsList = new List<Employee>(juniors);
        
        var teamLeadsWishlistsList = new List<Wishlist>(teamLeadsWishlists);
        var juniorsWishlistsList = new List<Wishlist>(juniorsWishlists);
        
        var freeTeamLeads = new bool[teamLeadsList.Count + 1];
        var freeJuniors = new bool[juniorsList.Count + 1];
        for (var i = 1; i < freeTeamLeads.Length; i++)
            freeTeamLeads[i] = true;
        for (var i = 1; i < freeJuniors.Length; i++)
            freeJuniors[i] = true;
        
        var teams = new List<Team>();
        
        
        // Пока есть свободный лид
        while (freeTeamLeads.Contains(true))
        {

            Employee? freeTeamLead = teamLeadsList.Find(t => t.Id == Array.FindIndex(freeTeamLeads, f => f));
            Debug.Assert(freeTeamLead != null, nameof(freeTeamLead) + " != null");
            
            Wishlist? freeTeamLeadsWishlist = teamLeadsWishlistsList.Find(w => w.EmployeeId == freeTeamLead.Id);
            Debug.Assert(freeTeamLeadsWishlist != null, nameof(freeTeamLeadsWishlist) + " != null");
            
            int mostWantedJuniorId = freeTeamLeadsWishlist.DesiredEmployees.First();
            
            Employee? mostWantedJunior = juniorsList.Find(j => j.Id == mostWantedJuniorId);
            Debug.Assert(mostWantedJunior != null, nameof(mostWantedJunior) + " != null");
            
            // Если выбранный джун свободен, создаем команду
            if (freeJuniors[mostWantedJuniorId] == true)
            {   
                teams.Add(new Team(freeTeamLead, mostWantedJunior));
                
                freeTeamLeads[freeTeamLead.Id] = false;
                freeJuniors[mostWantedJuniorId] = false;
                
                continue;
            }
            
            // Смотрим с кем в команде выбранный джун
            Employee? currentTeamLead =
                teams.Find(t => t.Junior.Id == mostWantedJuniorId)?.TeamLead;
            Debug.Assert(currentTeamLead != null, nameof(currentTeamLead) + " != null");
            
            Wishlist? wishlistOfMostWantedJunior = juniorsWishlistsList.Find(w => w.EmployeeId == mostWantedJuniorId);
            Debug.Assert(wishlistOfMostWantedJunior != null, nameof(wishlistOfMostWantedJunior) + " != null");
            
            // Находим приоритеты выбранного (свободного) лида и лида, с которым джун в команде
            int freeTeamLeadPriority = 20 - Array.IndexOf(wishlistOfMostWantedJunior.DesiredEmployees, freeTeamLead.Id);
            int currentTeamLeadPriority = 20 - Array.IndexOf(wishlistOfMostWantedJunior.DesiredEmployees, currentTeamLead.Id);

            // если джун уже состоит в команде, но выбранный лид имеет более высокий приоритет, чем сокомандник джуна
            if (freeJuniors[mostWantedJuniorId] == false &&
                freeTeamLeadPriority > currentTeamLeadPriority)
            {
                // Создаем команду из джуна и выбранного лида
                Team? removedTeam = teams.Find(t => t.Junior.Id == mostWantedJuniorId);
                Debug.Assert(removedTeam != null, nameof(removedTeam) + " != null");
                teams.Remove(removedTeam);
                teams.Add(new Team(freeTeamLead, mostWantedJunior));
                
                // Обновляем их статус занятости
                freeTeamLeads[freeTeamLead.Id] = false;
                freeJuniors[mostWantedJuniorId] = false;

                // Удаляем джуна из вишлиста его прошлого сокомандника (лида)
                int[]? currentTeamLeadWishlistIds = teamLeadsWishlistsList.Find(w => w.EmployeeId == currentTeamLead.Id)?.DesiredEmployees;
                Debug.Assert(currentTeamLeadWishlistIds != null, nameof(currentTeamLeadWishlistIds) + " != null");
                
                int[] newCurrentTeamLeadWishlistIds = currentTeamLeadWishlistIds.Where(id => id != Array.IndexOf(currentTeamLeadWishlistIds, mostWantedJunior.Id)).ToArray();
                Wishlist newCurrentTeamLeadWishlist = new Wishlist(currentTeamLead.Id, newCurrentTeamLeadWishlistIds);
                teamLeadsWishlistsList[teamLeadsWishlistsList.FindIndex(w => w.EmployeeId == currentTeamLead.Id)] =
                    newCurrentTeamLeadWishlist;
                
                // Обновляем статус занятости прошлого сокомандника джуна на 'свободен'
                freeTeamLeads[currentTeamLead.Id] = true;
                
                continue;
            }
            
            // Удаляем джуна из вишлиста выбранного (свободного) лида
            int[]? freeTeamLeadWishlistIds = teamLeadsWishlistsList.Find(w => w.EmployeeId == freeTeamLead.Id)?.DesiredEmployees;
            Debug.Assert(freeTeamLeadWishlistIds != null, nameof(freeTeamLeadWishlistIds) + " != null");
            
            int[] newFreeTeamLeadWishlistIds = freeTeamLeadWishlistIds.Where((v, id) => id != Array.IndexOf(freeTeamLeadWishlistIds, mostWantedJuniorId)).ToArray();

            Wishlist newFreeTeamLeadWishlist = new Wishlist(freeTeamLead.Id, newFreeTeamLeadWishlistIds);
            
            int wishlistIdx = teamLeadsWishlistsList.FindIndex(w => w.EmployeeId == freeTeamLead.Id);
            teamLeadsWishlistsList[wishlistIdx] = newFreeTeamLeadWishlist;
        }
        
        return teams;
    }
}