namespace LibraryDev.Application.Queries.Relatorios;

public class ObterRelatorioLivrosLidosQuery
{
    public int Ano { get; set; }

    public ObterRelatorioLivrosLidosQuery(int ano)
    {
        Ano = ano;
    }
}
