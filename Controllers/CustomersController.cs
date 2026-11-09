using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsValidation.Models;
using ProductsValidation.Services;
namespace ProductsValidation.Controllers {
public class CustomersController : Controller {
 readonly ShoppingContext db;
 public CustomersController(ShoppingContext context) {db=context;}
 public async Task<IActionResult> Index(string search,string sort,int page=1) { var query=db.Customers.AsQueryable();
 if(!string.IsNullOrWhiteSpace(search)) query=query.Where(c=>c.FirstName.Contains(search)||c.LastName.Contains(search));
 query=sort switch {"name_desc"=>query.OrderByDescending(c=>c.LastName),"address"=>query.OrderBy(c=>c.Address),"address_desc"=>query.OrderByDescending(c=>c.Address),_=>query.OrderBy(c=>c.LastName)};
 ViewBag.Search=search; ViewBag.Sort=sort;
 return View(await query.ToListAsync()); }
 public async Task<IActionResult> Details(int id) {var item=await db.Customers.FindAsync(id);return item==null?NotFound():View(item);}
 public IActionResult Create() => View("Edit",new Customer());
 public async Task<IActionResult> Edit(int id) {var item=await db.Customers.FindAsync(id);return item==null?NotFound():View(item);}
 [HttpPost,ValidateAntiForgeryToken]
 public async Task<IActionResult> Create([Bind("FirstName,LastName,Address,Discount")] Customer item) {
 if(!ModelState.IsValid) return View("Edit",item);
 db.Customers.Add(item);await db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
 [HttpPost,ValidateAntiForgeryToken]
 public async Task<IActionResult> Edit(int id,[Bind("FirstName,LastName,Address,Discount")] Customer item) {
 var current=await db.Customers.FindAsync(id);if(current==null)return NotFound();
 item.Id=id;if(!ModelState.IsValid)return View(item);
 current.FirstName=item.FirstName;current.LastName=item.LastName;current.Address=item.Address;current.Discount=item.Discount;
 await db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
 public async Task<IActionResult> Delete(int id) {var item=await db.Customers.FindAsync(id);return item==null?NotFound():View(item);}
 [HttpPost,ActionName("Delete"),ValidateAntiForgeryToken]
 public async Task<IActionResult> DeleteConfirmed(int id) {
 var item=await db.Customers.FindAsync(id);if(item==null)return NotFound();
 if(await db.Orders.AnyAsync(o=>o.CustomerId==id)) {ModelState.AddModelError("","Запис використовується в замовленні.");return View("Delete",item);}
 db.Customers.Remove(item);await db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
}
}