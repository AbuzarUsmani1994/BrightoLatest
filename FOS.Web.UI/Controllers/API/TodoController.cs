using FOS.DataLayer;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class TodoController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult GetRegionalHeadTodos(int regionalHeadId)
        {
            try
            {
                var list = db.Todoes.Where(d => d.RegionalHeadId == regionalHeadId && d.Status == (int)StatusEnum.Pending).ToList();
                if (list != null)
                {
                    return Ok(new
                    {
                        Todos = list.Select(d => new
                        {
                            d.TodoID,
                            d.Title,
                            d.Detail,
                            d.DueDate,
                            //DueDateLong = d.DueDate.Ticks,
                            d.Priority,
                            d.Status,
                            d.Remarks
                        }).OrderBy(d => d.DueDate)
                    });
                }
                else
                {
                    return Ok(new
                    {
                        Todos = new { }
                    });
                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "GetRegionalHeadTodos List API Failed");
                return Ok(new
                {
                    Todos = new { }
                });
            }
        }
        [System.Web.Http.HttpPost]
        public IHttpActionResult UpdateTodo(TodoModel model)
        {
            try
            {
                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(model.Token, true))
                {
                    Todo todo = db.Todoes.Where(u => u.TodoID == model.TodoId).FirstOrDefault();

                    if (todo != null)
                    {
                        todo.Remarks = model.Remarks;
                        todo.UpdatedBy = model.UpdatedBy;
                        todo.RemUpdatedOn = DateTime.Now;

                        // Add Token Detail ...
                        TokenDetail tokenDetail = new TokenDetail();
                        tokenDetail.TokenName = model.Token;
                        tokenDetail.Action = "Update TODO";
                        tokenDetail.ProcessedDateTime = DateTime.Now;
                        db.TokenDetails.Add(tokenDetail);
                        //END
                        db.SaveChanges();
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = new CheckInLatLongResp
                            {
                                code = "400",
                                message = "Todo not found in update TODO API"
                            }
                        });
                    }
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "200",
                            message = "Todo updated successfully"
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "400",
                            message = "Authentication failed in update todo API"
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Update Todo API Failed");
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "Update Todo API Failed"
                    }
                });
            }
        }
    }
    public class TodoModel
    {
        public int TodoId { get; set; }
        public string Remarks { get; set; }
        public int UpdatedBy { get; set; }
        public string Token { get; set; }
    }
}