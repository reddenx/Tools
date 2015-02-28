﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using SMT.Utilities.Sql.Interfaces;
using SMT.Utilities.Sql.SqlCe;
using Web.TaskTracker.Models.Bol;

namespace Web.TaskTracker.Models.Dal
{
    public class TaskRepository
    {
        private readonly ISqlQuerier Querier;

        public TaskRepository(ISqlQuerier querier = null)
        {
            this.Querier = querier ?? SqlCeQuerier.Get(DataConfiguration.TaskConnectionString);
        }

        public IEnumerable<TaskItem> GetTasksForAccount(int accountId)
        {
            var sql = 
@"select t.*
from Task t
where t.AccountId = @AccountId";

            var parameters = new IDbDataParameter[] 
            {
                Querier.CreateParameter("@AccountId", SqlDbType.Int, accountId),
            };

            return Querier.ExecuteReader(BuildTaskFromReader, sql, parameters);
        }

        public int CreateTask(string taskName, int accountId, int? parentTaskId)
        {
            var sql =
@"insert into Task (AccountId, ParentTaskId, TaskName, CurrentStatusId, DateCreated, DateCompleted)
values (@AccountId,@ParentTaskId,@TaskName,@CurrentTaskId,GETDATE(),null)";

            var parameters = new IDbDataParameter[]
            {
                Querier.CreateParameter("@AccountId", SqlDbType.Int, accountId),
                Querier.CreateParameter("@ParentTaskId", SqlDbType.Int, parentTaskId),
                Querier.CreateParameter("@TaskName", SqlDbType.NVarChar, 255, taskName),
                Querier.CreateParameter("@CurrentTaskId", SqlDbType.Int, TaskStatus.Active),
            };

            return Querier.InsertAndGetIdentity(sql, parameters);
        }

        public TaskItem GetTaskById(int taskId)
        {
            var sql =
@"select t.*
from Task t
where t.TaskId = @TaskId";

            var parameters = new IDbDataParameter[] 
            {
                Querier.CreateParameter("@TaskId", SqlDbType.Int, taskId),
            };

            return Querier.ExecuteReader(BuildTaskFromReader, sql, parameters).Single();
        }

        public void UpdateTask(int taskId, string taskName, TaskStatus currentStatus)
        {
            var sql =
@"update Task
set TaskName = @TaskName, CurrentStatusId = @CurrentStatusId
where TaskId = @TaskId";

            var parameters = new IDbDataParameter[] 
            {
                Querier.CreateParameter("@TaskId", SqlDbType.Int, taskId),
                Querier.CreateParameter("@TaskName", SqlDbType.NVarChar, 255, taskName),
                Querier.CreateParameter("@CurrentStatusId", SqlDbType.Int, currentStatus),
            };

            Querier.ExecuteNonQuery(sql, parameters);
        }

        private TaskItem BuildTaskFromReader(IDataReader reader)
        {
            return new TaskItem(
                    Convert.ToInt32(reader["TaskId"]),
                    Convert.ToInt32(reader["AccountId"]),
                    reader["ParentTaskId"] == DBNull.Value ? null : new Nullable<int>(Convert.ToInt32(reader["ParentTaskId"])),
                    Convert.ToString(reader["TaskName"]),
                    (TaskStatus)Convert.ToInt32(reader["CurrentStatusId"]),
                    Convert.ToDateTime(reader["DateCreated"]),
                    reader["DateCompleted"] == DBNull.Value ? null : new Nullable<DateTime>(Convert.ToDateTime(reader["DateCompleted"])));
        }
    }
}