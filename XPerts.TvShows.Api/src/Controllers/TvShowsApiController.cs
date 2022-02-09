using Microsoft.AspNetCore.Mvc;
using System.Net;
using XPerts.TvShows.Models;
using XPertz.TvShows.Controllers;

namespace XPerts.TvShows.Api.Controllers
{
    /// <summary>
    /// Api controller for peforming the basic <see cref="XPerts.TvShows.Models.TvShow"/> CRUD operations.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    [Route("shows")]
    public class TvShowsApiController : ControllerBase
    {
        private readonly ILogger<TvShowsApiController> _logger;
        private readonly IDataController<TvShow, TvShowView> _dataController;

        /// <summary>
        /// Initializes a new instance of the <see cref="TvShowsApiController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dataController">The data controller.</param>
        public TvShowsApiController(ILogger<TvShowsApiController> logger, IDataController<TvShow, TvShowView> dataController)
        {
            _logger = logger;
            _dataController = dataController;
        }

        /// <summary>
        /// Gets all the tv shows on a specified page. If the page query parameter is not specified, the default of page 1 is used.
        /// </summary>
        /// <returns>
        /// A collection of tv shows.
        /// </returns>
        [HttpGet(Name = "GetTvShows")]
        public async Task<ActionResult<IPage<TvShowView>>> GetAsync([FromQuery] int page = 1)
        {
            try
            {
                var result = await _dataController
                    .GetAsync(page)
                    .ConfigureAwait(false);

                return Ok(result);
            }
            catch (ExceptionResult e)
            {
                return StatusCode((int)e.StatusCode, new { error = e.Message });
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        /// <summary>
        /// Gets the tv show with the specified id asynchronously.
        /// </summary>
        /// <returns>
        /// The tv show with the specified id.
        /// </returns>
        [HttpGet("{id:long}", Name = "GetTvShowById")]
        public async Task<ActionResult<TvShowView>> GetByIdAsync(long id)
        {
            try
            {
                if (id == 0)
                    return BadRequest(new { error = "Please specify a valid id that is greater or equal to 1." });

                var result = await _dataController
                    .GetAsync(id)
                    .ConfigureAwait(false);

                return Ok(result);
            }
            catch (ExceptionResult e)
            {
                return StatusCode((int)e.StatusCode, new { error = e.Message });
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        /// <summary>
        /// Adds the specified tv show asynchronously. When genres are provided that do not yet exists, then adds those genres as well.
        /// </summary>
        /// <param name="show">The show.</param>
        /// <returns>
        /// The added tv show.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<TvShowView>> AddAsync([FromBody] TvShowView show)
        {
            try
            {
                if (show.Id > 0)
                    show.Id = 0;

                var result = await _dataController
                    .AddAsync(show)
                    .ConfigureAwait(false);

                return Ok(result);
            }
            catch (ExceptionResult e)
            {
                return StatusCode((int)e.StatusCode, new { error = e.Message });
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        /// <summary>
        /// Updates the properties specified in the given tv show asynchronously. When genres are specified, and they do not exist yet, then creates these genres.
        /// Note that all the provided genres will overwrite any previously assigned genres. Be sure that if old genres are meant to remain, to include these in
        /// the request as well.
        /// </summary>
        /// <param name="id">The tv show identifier.</param>
        /// <param name="show">The tv show with properties to update.</param>
        /// <returns>
        /// The updated tv show.
        /// </returns>
        [HttpPost("{id:long}")]
        public async Task<ActionResult<TvShowView>> UpdateAsync(long id, [FromBody] TvShowView show)
        {
            if (id <= 0)
                return BadRequest(new { error = "Please specify a valid id that is greater or equal to 1" });

            try
            {
                show.Id = id;

                var result = await _dataController
                    .UpdateAsync(show)
                    .ConfigureAwait(false);

                return Ok(result);
            }
            catch (ExceptionResult e)
            {
                return StatusCode((int)e.StatusCode, new { error = e.Message });
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        private ObjectResult InternalServerError(Exception e)
        {
            var eventId = Guid.NewGuid().ToString();
            _logger.LogError(new EventId(666, eventId), e, "Failed to execute");
            return StatusCode((int)HttpStatusCode.InternalServerError, new
            {
                error = $"An unexpected error occurred. Please contact the application admin and provide him or her with the following correlation id '{eventId}'."
            });
        }
    }
}