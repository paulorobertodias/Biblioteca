using System;
using System.Collections.Generic;
using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    public class LivroController : Controller
    {
        public IActionResult Cadastro()
        {
            Autenticacao.CheckLogin(this);
            return View();
        }

        [HttpPost]
        public IActionResult Cadastro(Livro l)
        {
            if(!string.IsNullOrEmpty(l.Titulo) && !string.IsNullOrEmpty(l.Autor) && l.Ano!=0)
            {
                LivroService livroService = new LivroService();

                if(l.Id == 0)
                {
                    livroService.Inserir(l);
                }
                else
                {
                    livroService.Atualizar(l);
                }

                return RedirectToAction("Listagem");
            }
            else
            {
                ViewData["mensagem"] = "Preencha todos os campos";
                return View();
            }
        }

        public IActionResult Listagem(string TipoFiltro, string Filtro, int p = 1)
        {
            Autenticacao.CheckLogin(this);
            FiltrosLivros objFiltro = null;
            if(!string.IsNullOrEmpty(Filtro))
            {
                objFiltro = new FiltrosLivros();
                objFiltro.Filtro = Filtro;
                objFiltro.TipoFiltro = TipoFiltro;
            }

/*            ViewData["livrosPorPagina"] = (string.IsNullOrEmpty(itensPorPagina) ? 10 : Int32.Parse(itensPorPagina));
            ViewData["PaginaAtual"] = (PaginaAtual!=0 ? PaginaAtual : 1);*/
            int quantidadePorPagina = 10;

            LivroService livroService = new LivroService();
            int totalDeRegistros = livroService.NumeroDeLivros();
            ICollection<Livro> lista = livroService.ListarTodos(p, quantidadePorPagina, objFiltro);
            ViewData["NroPaginas"] = (int) Math.Ceiling((double) totalDeRegistros / quantidadePorPagina);
            return View(lista);

        }

        public IActionResult Edicao(int id)
        {
            Autenticacao.CheckLogin(this);
            LivroService ls = new LivroService();
            Livro l = ls.ObterPorId(id);
            return View(l);
        }
    }
}