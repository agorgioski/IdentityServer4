using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;

using Models;
using Data.AutoMapper;
using System.IO;

public class CaseRepository : ICaseRepository
{
    private readonly string _localDbConnectionString;

    public CaseRepository(string connectionString)
    {
        if (String.IsNullOrEmpty(connectionString)) 
        {
            throw new Exception("Empty connection string");
        }
        _localDbConnectionString = connectionString;
        Mapper.initMapper();
    }
    public async Task<List<Case>> GetAllCases()
    {
        List<Case> cases = null;
        using (SqlConnection conn = new SqlConnection(_localDbConnectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand("CaseGetAll", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            cases = Mapper.mapper.Map<IDataRecord, IEnumerable<Case>>(reader).ToList();
                            // var test = reader.GetString(reader.GetOrdinal("CaseKindId"));
                        }
                    }
                }
            }
        }
        return cases;
    }

    public async Task<Case> GetCaseById(long id)
    {
        Case @case = null;
        using (SqlConnection conn = new SqlConnection(_localDbConnectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand("CaseGetById", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@CaseId", id));
                
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            @case = Mapper.mapper.Map<IDataRecord, Case>(reader);
                        }
                    }
                }
            }
        }
        return @case;
    }

    public async Task InsertUpdateCase(Case @case, long id)
    {
        using (SqlConnection conn = new SqlConnection(_localDbConnectionString))
        {
            await conn.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("CaseInsertUpdate", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@CaseId", (id != -1 ? (object)id : DBNull.Value)));
                cmd.Parameters.Add(new SqlParameter("@Description", @case.Description));
                cmd.Parameters.Add(new SqlParameter("@Status", @case.Status));
                cmd.Parameters.Add(new SqlParameter("@Title", @case.Title));
                cmd.Parameters.Add(new SqlParameter("@CaseNumber", @case.CaseNumber));
                cmd.Parameters.Add(new SqlParameter("@CaseKindType", @case.Kind.Type));

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
    public async Task DeleteCase(long id)
    {
        using (SqlConnection conn = new SqlConnection(_localDbConnectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand("CaseDelete", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@CaseId", id));
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
    public async Task<CaseKind> GetCaseKindByType(string type)
    {
        CaseKind caseKind = null;
        using (SqlConnection conn = new SqlConnection(_localDbConnectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand("CaseKindGetByType", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@CaseKindType", type));

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            caseKind = Mapper.mapper.Map<IDataRecord, CaseKind>(reader);
                        }
                    }
                }
            }
        }
        return caseKind;
    }
}