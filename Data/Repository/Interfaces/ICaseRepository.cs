using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

public interface ICaseRepository
{
    //get, get all, delete, update, create
    Task<List<Case>> GetAllCases();
    Task<Case> GetCaseById(long id);
    Task DeleteCase(long id);
    Task InsertUpdateCase(Case @case, long id);
    Task<CaseKind> GetCaseKindByType(string type);
}