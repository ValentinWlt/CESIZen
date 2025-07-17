#!/bin/bash

# Script de déploiement pour CESIZen
# Usage: ./deploy.sh [start|stop|restart|build|logs|clean]

set -e

# Couleurs pour les messages
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Fonction pour afficher les messages colorés
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Vérifier si Docker est installé
check_docker() {
    if ! command -v docker &> /dev/null; then
        log_error "Docker n'est pas installé ou non accessible"
        exit 1
    fi
    
    if ! command -v docker-compose &> /dev/null && ! docker compose version &> /dev/null; then
        log_error "Docker Compose n'est pas installé ou non accessible"
        exit 1
    fi
}

# Construire les images
build() {
    log_info "Construction des images Docker..."
    docker-compose build --no-cache
    log_success "Images construites avec succès"
}

# Démarrer les services
start() {
    log_info "Démarrage de CESIZen..."
    docker-compose up -d
    
    log_info "Attente du démarrage des services..."
    sleep 10
    
    # Vérifier l'état des services
    if docker-compose ps | grep -q "Up"; then
        log_success "CESIZen démarré avec succès"
        log_info "Application disponible sur:"
        log_info "  - HTTP:  http://localhost:8080"
        log_info "  - HTTPS: https://localhost:8081"
        log_info "Base de données SQL Server sur le port 1433"
    else
        log_error "Erreur lors du démarrage"
        docker-compose logs
    fi
}

# Arrêter les services
stop() {
    log_info "Arrêt de CESIZen..."
    docker-compose down
    log_success "CESIZen arrêté"
}

# Redémarrer les services
restart() {
    log_info "Redémarrage de CESIZen..."
    stop
    start
}

# Afficher les logs
logs() {
    log_info "Affichage des logs..."
    docker-compose logs -f
}

# Nettoyer (arrêter et supprimer les volumes)
clean() {
    log_warning "Cette action va supprimer tous les volumes et données !"
    read -p "Êtes-vous sûr ? (y/N): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        log_info "Nettoyage en cours..."
        docker-compose down -v
        docker system prune -f
        log_success "Nettoyage terminé"
    else
        log_info "Nettoyage annulé"
    fi
}

# Vérifier l'état des services
status() {
    log_info "État des services CESIZen:"
    docker-compose ps
}

# Migration de la base de données
migrate() {
    log_info "Application des migrations de base de données..."
    docker-compose exec cesizen-app dotnet ef database update
    log_success "Migrations appliquées"
}

# Fonction principale
main() {
    check_docker
    
    case "${1:-start}" in
        start)
            start
            ;;
        stop)
            stop
            ;;
        restart)
            restart
            ;;
        build)
            build
            ;;
        logs)
            logs
            ;;
        clean)
            clean
            ;;
        status)
            status
            ;;
        migrate)
            migrate
            ;;
        *)
            echo "Usage: $0 {start|stop|restart|build|logs|clean|status|migrate}"
            echo ""
            echo "Commandes disponibles:"
            echo "  start    - Démarrer CESIZen"
            echo "  stop     - Arrêter CESIZen"
            echo "  restart  - Redémarrer CESIZen"
            echo "  build    - Construire les images Docker"
            echo "  logs     - Afficher les logs en temps réel"
            echo "  clean    - Arrêter et nettoyer (supprime les données)"
            echo "  status   - Afficher l'état des services"
            echo "  migrate  - Appliquer les migrations de base de données"
            exit 1
            ;;
    esac
}

main "$@"