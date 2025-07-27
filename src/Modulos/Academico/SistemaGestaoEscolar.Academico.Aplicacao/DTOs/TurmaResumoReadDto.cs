
namespace SistemaGestaoEscolar.Academico.Aplicacao.DTOs
{
    public class TurmaResumoReadDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public string Turno { get; set; } = string.Empty;
        public int AnoLetivo { get; set; }
        public int AlunosMatriculados { get; set; }
        public int CapacidadeMaxima { get; set; }
        public int VagasDisponiveis => CapacidadeMaxima - AlunosMatriculados;
        public bool Ativa { get; set; }
    }
}
