using Cards.API.Data;
using Cards.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace Cards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardsController : Controller
    {


        private readonly CardsDbContext cardsDbContext;
        public CardsController(CardsDbContext cardsDbContext)
        {
            this.cardsDbContext = cardsDbContext;
        }

        //ECDiffieHellman between IActionResult and async Task<IactionResult>
        //DbContext injected in constructor constructor injection to call the DB CardsDbContext
        //Get All cards
        [HttpGet]
        public async Task<IActionResult> GetAllCards()
        {

            //var insert = cardsDbContext.Database.ExecuteSqlRaw($"InsertJson {"abcd"}");
            var cards = await cardsDbContext.Cards.ToListAsync();
            return Ok(cards);
        }

        //Get Single Card
        [HttpGet]
        [Route("{id:guid}")]// parameter curly braces //To aceept only guid type   
        [ActionName("GetCard")]// To be called from CreatedAtAction after creating single card and fetching it
        public async Task<IActionResult> GetCard([FromRoute] Guid id)
        {
            var cards = await cardsDbContext.Cards.FirstOrDefaultAsync(x=> x.Id == id);
            if(cards != null)
            {
                return Ok(cards);
            }

            return NotFound("Card not found");
        }


        [HttpPost]     
        public async Task<IActionResult> AddCard([FromBody] Card card)//Api model Card
        {
            card.Id = Guid.NewGuid();

          await  cardsDbContext.Cards.AddAsync(card);
            await cardsDbContext.SaveChangesAsync();
            //return CreatedAtAction(nameof(GetCard), card.Id, card);
            return CreatedAtAction(nameof(GetCard), new { id = card.Id}, card); //as an object

            //ReturnTypeEncoder 2 o 1 response -> create header with proper location to fetch the created card
            //It won't add directly to Database -> It only saves to reference ->So to save call card Db Context to save changes async


        }

        //UPdating a card
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateCard([FromRoute] Guid id, [FromBody] Card card)
        {//FromRoute id of card we want to change and From body updated detail data to change it into

            var existingCard = await cardsDbContext.Cards.FirstOrDefaultAsync(x => x.Id == id);
            if(existingCard != null)
            {
                existingCard.CardholderName = card.CardholderName;
                existingCard.CardNumber = card.CardNumber;
                existingCard.ExpiryMonth = card.ExpiryMonth;
                existingCard.ExpiryYear = card.ExpiryYear;
                existingCard.CVC = card.CVC;
                await cardsDbContext.SaveChangesAsync();
                return Ok(existingCard);
            } 
                return NotFound("Card Not Found");
           

        }

        //Delete a card
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteCard([FromRoute] Guid id)
        {//FromRoute id of card we want to change and From body updated detail data to change it into

            var existingCard = await cardsDbContext.Cards.FirstOrDefaultAsync(x => x.Id == id);
            if (existingCard != null)
            {
                cardsDbContext.Remove(existingCard);
                await cardsDbContext.SaveChangesAsync();
                return Ok(existingCard);
            } 
                return NotFound("Card Not Found");
       

        }

    }
}
