using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Seikatsu.Backend.CommonAPIRespone;
using Seikatsu.Backend.Models;
using Seikatsu.Backend.Services;
using System.Security.Claims;

namespace Seikatsu.Backend.Controllers
{
    [EnableCors("specificOrigins")]
    [ApiController]
    [Route("api/[controller]")]
    public class ChecklistController(IChecklistService checklistService) : ControllerBase
    {
        [Authorize]
        [HttpGet("getchecklist")]

        public async Task<ActionResult<APIResponse<GetChecklistDTO>>> GetCheckList()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await checklistService.GetCheckListAsync(customerId);
            var response = new APIResponse<GetChecklistDTO>
            {
                Success = true,
                Data = result,
                Message = result != null ? "Products in checklist are sent" : "No address found."
            };
            return Ok(response);


        }

        [Authorize]
        [HttpPost("additems")]

        public async Task<ActionResult<APIResponse<AddItemstoChecklistResponseDTO>>> AddItemsToChecklist(AddItemstoChecklistRequestDTO request)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await checklistService.AddItemstoChecklistAsync(customerId, request);
            var response = new APIResponse<AddItemstoChecklistResponseDTO>
            {
                Success = true,
                Data = result,
                Message = "Products added to checklist successfully."
            };
            return Ok(response);
        }

        [Authorize]
        [HttpPost("additem")]

        public async Task<ActionResult<APIResponse<CheckListItemResponseDTO>>> AddSingleItemToChecklist(CheckListItemDTO request)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await checklistService.AddSingleItemAysnc(customerId, request);
            var response = new APIResponse<CheckListItemResponseDTO>
            {
                Success = true,
                Data = result,
                Message = "Product added to checklist successfully."
            };
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("deleteitem/{itemId}")]
        public async Task<ActionResult<APIResponse<bool>>> DeleteItemFromChecklist([FromRoute] Guid itemId)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await checklistService.DeleteItemAsync(customerId, itemId);
            var response = new APIResponse<bool>
            {
                Success = result,
                Data = result,
                Message = result ? "Product deleted from checklist successfully." : "Failed to delete product from checklist."
            };
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("deletechecklist")]
        public async Task<ActionResult<APIResponse<bool>>> DeleteChecklist()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await checklistService.DeleteCheckListAsync(customerId);
            var response = new APIResponse<bool>
            {
                Success = result,
                Data = result,
                Message = result ? "Checklist deleted successfully." : "Failed to delete checklist."
            };
            return Ok(response);

        }

        [Authorize]
        [HttpPatch("modifyitems")]
        //for multiple items modification
  
        public async Task<ActionResult<APIResponse<ModifyItemsResponseDTO>>> ModifyItemsInChecklist(ModifyItemsinChecklistRequestDTO request)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await checklistService.ModifyProductAsync(customerId, request);
            var response = new APIResponse<ModifyItemsResponseDTO>
            {
                Success = true,
                Data = result,
                Message = "Products in checklist modified successfully."
            };
            return Ok(response);
        }

        [Authorize]
        [HttpPatch("modifyitem")]
        //for single item modification
        public async Task<ActionResult<APIResponse<CheckListItemResponseDTO>>> ModifySingleItemInChecklist(Guid itemId, CheckListItemDTO request)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await checklistService.ModifySingleProductAsync(customerId, itemId, request);
            var response = new APIResponse<CheckListItemResponseDTO>
            {
                Success = true,
                Data = result,
                Message = "Product in checklist modified successfully."
            };
            return Ok(response);

        }
    }


}