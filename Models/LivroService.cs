using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Biblioteca.Models
{
    public class LivroService
    {
        public void Inserir(Livro l)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                bc.Livros.Add(l);
                bc.SaveChanges();
            }
        }

        public void Atualizar(Livro l)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                Livro livro = bc.Livros.Find(l.Id);
                livro.Autor = l.Autor;
                livro.Titulo = l.Titulo;
                livro.Ano = l.Ano;

                bc.SaveChanges();
            }
        }

        public ICollection<Livro> ListarTodos( int pagina = 1, int tamanho = 10, FiltrosLivros filtro = null)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                IQueryable<Livro> query;
                
                int pular = (pagina - 1) * tamanho;
                
                if(filtro != null)
                {
                    //definindo dinamicamente a filtragem
                    switch(filtro.TipoFiltro)
                    {
                        case "Autor":
                            query = bc.Livros.Where(l => l.Autor.Contains(filtro.Filtro, System.StringComparison.CurrentCultureIgnoreCase));
                        break;

                        case "Titulo":
                            query = bc.Livros.Where(l => l.Titulo.Contains(filtro.Filtro, System.StringComparison.CurrentCultureIgnoreCase));
                        break;

                        default:
                            query = bc.Livros;
                            break;
                    }
                }
                else
                {
                    // caso filtro não tenha sido informado
                    query = bc.Livros;
                }
                
                //ordenação padrão
                return query.OrderBy(l => l.Titulo).Skip (pular).Take (tamanho).ToList();
            }
        }

        public ICollection<Livro> ListarDisponiveis()
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                //busca os livros onde o id não está entre os ids de livro em empréstimo
                // utiliza uma subconsulta
                return
                    bc.Livros
                    .Where(l =>  !(bc.Emprestimos.Where(e => e.Devolvido == false).Select(e => e.LivroId).Contains(l.Id)) )
                    .ToList();
            }
        }

        public Livro ObterPorId(int id)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                return bc.Livros.Find(id);
            }
        }

        public int NumeroDeLivros()
        {
             using( var Context = new BibliotecaContext())
             return Context.Livros.Count();
        }
    }
}