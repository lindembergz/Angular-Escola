
namespace SistemaGestaoEscolar.Academico.Aplicacao.DTOs
{
    public class TurmaResumoReadDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public int Serie { get; set; }
        public string Turno { get; set; }
        public int AlunosMatriculados { get; set; }
        public int CapacidadeMaxima { get; set; }
    }
}
