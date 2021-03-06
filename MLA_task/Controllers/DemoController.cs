﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using MLA_task.EF;
using NLog;

namespace MLA_task.Controllers
{
    public class DemoController : ApiController
    {
        private readonly ILogger _logger;
        private readonly DemoContext _context;

        public DemoController(ILogger logger, DemoContext demoContext)
        {
            _logger = logger;
            _context = demoContext;
        }

        public async Task<IHttpActionResult> Get()
        {
            var models = await _context.DemoDbModels.ToListAsync();

            return Ok(models.Select(model => new { Id = model.Id, Name = model.Name, InfoId = model.DemoCommonInfoModelId, Info = model.DemoCommonInfoModel.CommonInfo }));
        }

        // GET: Demo
        public async Task<IHttpActionResult> Get(int id)
        {
            _logger.Info($"receiving item with id {id}");

            if (id == 23) {
                _logger.Info($"Wrong ID {id} has been requested");
                return this.BadRequest("Wrong ID");
            }

            try {
                var model = await _context.DemoDbModels.SingleAsync(item => item.Id == id);

                _logger.Info($"item with id {id} has been received.");

                return Ok(new { Id = model.Id, Name = model.Name, InfoId = model.DemoCommonInfoModelId, Info = model.DemoCommonInfoModel.CommonInfo });
            } catch (Exception ex) {
                _logger.Error(ex, $"Server error occured while trying to get item with id {id}");
                return this.InternalServerError();
            }
        }

        public async Task<IHttpActionResult> Post([FromBody]DemoModel model)
        {
            _logger.Info($"adding model with name {model.Name}");

            if (model.Name == "bla-bla") {
                _logger.Info($"Wrong model name {model.Name} detected");
                return this.BadRequest("WrongName");
            }

            try {
                _context.DemoDbModels.Add(model);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                _logger.Error(ex, $"Server error occured while trying to add item with name {model.Name}");
                return this.InternalServerError();
            }
           
            return Ok();
        }
    }
}