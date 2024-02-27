using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace JourneyPlanner.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JourneyPlannerController : ControllerBase
    {
        private readonly ILogger<JourneyPlannerController> Logger;

        public JourneyPlannerController(ILogger<JourneyPlannerController> logger)
        {
            Logger = logger;
        }



        /// <summary>
        /// This is intended to take a 'short list' of possible Journeys 
        /// which have already been filtered, such as by some shortest path alogritms. 
        /// The required algorithm is specified in the query parameter and the journey list is provieded in the body json. 
        /// </summary>  
        /// <param name="journeys"></param> 
        /// <param name="algorithmName"></param> 
        /// <returns> List<Journey> which has been ordered by desirability most to least, according to specified algorithm. </returns>

        [HttpPost("desirability", Name = "GetJourneysDesirability")]
        public ActionResult GetJourneysDesirability( [FromBody] List<Journey> journeys, string algorithmName = "sam-emily") // Default Algorithm as optional param
        {
            JourneyAlgorithmFactory factory = new JourneyAlgorithmFactory();
            JourneyPlanner planner = new JourneyPlanner(factory);

            // Set the algorithm based on the name received from the web API
            planner.SetAlgorithm(algorithmName);

            try
            {
                List<Journey> desirableJourneys = planner.GetJourneysDesirability(journeys); 
                return Ok(desirableJourneys);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return StatusCode(500, $"An error occurred while processing your request to use the journey algorithm {algorithmName}");
            }

        }







    }
}
