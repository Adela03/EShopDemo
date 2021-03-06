using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EShopDemo.Models;
using EShopDemo.Data;
using System.Dynamic;
using Microsoft.AspNetCore.Identity;

namespace EShopDemo.Controllers
{
    public class CarritoController : Controller
    {
        private readonly ILogger<CarritoController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public CarritoController(ILogger<CarritoController> logger, ApplicationDbContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            if(_signInManager.IsSignedIn(User))
            {
                string email = User.Identity.Name;
                var user = _userManager.FindByEmailAsync(email);
                //se extrae el ID del usuario actualmente logueado
                var userId = _userManager.GetUserId(User);

                var listProductos=_context.Productos.ToList();
                var listCarro=_context.Carritos.ToList();
                var listMostrar= new List<Producto>();
                int total = 0;
                Producto prod= new Producto();
                dynamic model = new ExpandoObject();

                foreach(var carro in listCarro){
                    if(carro.user_id==userId){
                        for(int i=0; i<listProductos.Count; i++){
                            prod=listProductos[i];
                            if(prod.ID==carro.producto_id){
                                string imageBase64Data = Convert.ToBase64String(prod.Picture);
                                string imageDataURL = string.Format("data:image/jpg;base64,{0}",imageBase64Data);
                                ViewBag.imageDataURL = imageDataURL;
                                prod.imageData = ViewBag.imageDataURL;
                                total+=prod.Price;
                                listMostrar.Add(prod);
                                break;
                            }
                        }
                    }
                }

                model.Mostrar = listMostrar;
                model.Total = total;
                return View(model);
            }
            else
            {
                return LocalRedirect("/Identity/Account/Login");
            }
        }

        public IActionResult Delete(Producto producto){
            var listCarrito = _context.Carritos.ToList();
            var listProdUser = new List<Carrito>();
            string email = User.Identity.Name;
            var user = _userManager.FindByEmailAsync(email);
            var userId = _userManager.GetUserId(User);
            var carritoID=0;
            bool prodExist = false;

            for(int i=0; i<listCarrito.Count; i++){
                Carrito temp = listCarrito[i];
                if(userId==temp.user_id){
                    listProdUser.Add(temp);
                    //verifica si el ID de producto existe en la tabla
                    if(producto.ID==temp.producto_id){
                        prodExist=true;
                        carritoID=temp.ID;
                    }
                }
            }

            if(prodExist==true){
                Carrito carrito=_context.Carritos.Find(carritoID);
                _context.Carritos.Remove(carrito);
                _context.SaveChanges();
                return RedirectToAction("Index","Carrito");
            }else{
                return RedirectToAction("Index","Carrito");
            }
            
        }

    }
}