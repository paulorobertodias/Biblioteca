using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Models
{
    public class EmprestimoService 
    {
        public void Inserir(Emprestimo e)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                bc.Emprestimos.Add(e);
                bc.SaveChanges();
            }
        }

        public void Atualizar(Emprestimo e)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                Emprestimo emprestimo = bc.Emprestimos.Find(e.Id);
                emprestimo.NomeUsuario = e.NomeUsuario;
                emprestimo.Telefone = e.Telefone;
                emprestimo.LivroId = e.LivroId;
                emprestimo.DataEmprestimo = e.DataEmprestimo;
                emprestimo.DataDevolucao = e.DataDevolucao;

                bc.SaveChanges();
            }
        }

        public ICollection<Emprestimo> ListarTodos(int pagina = 1, int tamanho = 10, FiltrosEmprestimos filtro = null)// Filtro não utilizado
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                IQueryable<Emprestimo> consulta;

                int pular = (pagina - 1) * tamanho;
                
                if(filtro != null)
                {
                    //definindo dinamicamente a filtragem
                    switch(filtro.TipoFiltro)
                    {
                        case "Usuario":
                        //query = bc.Livros.Where(l => l.Autor.Contains(filtro.Filtro, System.StringComparison.CurrentCultureIgnoreCase));
                        consulta = bc.Emprestimos.Where(l => l.NomeUsuario.Contains(filtro.Filtro, System.StringComparison.CurrentCultureIgnoreCase));
                        break;

                        case "Livro":
                        //consulta = bc.Emprestimos.Where(l => l.Livro.Titulo.Contains(filtro.Filtro, System.StringComparison.CurrentCultureIgnoreCase));
                             //consulta = bc.Emprestimos.Where(e => bc.Livros Find(e.LivroId).Titulo.Contains(filtro.Filtro)); NÃO FUNCIONA
                             List<Livro> LivrosFiltrados = bc.Livros.Where(l => l.Titulo.Contains(filtro.Filtro)).ToList();

                             List<int> LivrosIds = new List<int>();
                             for (int i = 0; i < LivrosFiltrados.Count; i++)
                             {
                                 LivrosIds.Add(LivrosFiltrados[i].Id);
                             }
                             consulta = bc.Emprestimos.Where(e => LivrosIds.Contains(e.LivroId));
                             var debug = consulta.ToList();
                        break;

                        default:
                            consulta = bc.Emprestimos;
                            break;
                    }
                }
                else
                {
                    // caso filtro não tenha sido informado
                    consulta = bc.Emprestimos;
                }
                
                List<Emprestimo> ListaCosulta = consulta.OrderBy(e => e.DataEmprestimo).ToList();

                for (int i = 0; i < ListaCosulta.Count; i++)
                {
                    ListaCosulta[i].Livro = bc.Livros.Find(ListaCosulta[i].LivroId);
                }
                //ordenação padrão
                //return consulta.OrderByDescending(l => l.DataDevolucao).Skip (pular).Take (tamanho).ToList();
                return ListaCosulta.OrderByDescending(l => l.DataDevolucao).Skip (pular).Take (tamanho).ToList();
            }
        }
/*        public ICollection<Emprestimo> ListarTodos(FiltrosEmprestimos filtro)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                return bc.Emprestimos.Include(e => e.Livro).ToList();
            }
        }
*/
        public Emprestimo ObterPorId(int id)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                return bc.Emprestimos.Find(id);
            }
        }

         public int NumeroDeLivros()
        {
             using( var Context = new BibliotecaContext())
             return Context.Emprestimos.Count();
        }
    }
}