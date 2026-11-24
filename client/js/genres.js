const genresList = document.getElementById('genres');
const statusMessage = document.getElementById('status');
const reloadButton = document.getElementById('reload');

async function loadGenres() {
    reloadButton.disabled = true;
    genresList.replaceChildren();
    statusMessage.textContent = 'Зареждане...';
    try {
        const response = await fetch('http://localhost:5080/api/genres');
        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        const genres = await response.json();
        for (const genre of genres) {
            const item = document.createElement('li');
            item.textContent = genre.name;
            genresList.appendChild(item);
        }
        statusMessage.textContent = genres.length === 0
            ? 'Все още няма добавени жанрове.'
            : `Намерени жанрове: ${genres.length}`;
    } catch (error) {
        statusMessage.textContent = 'Жанровете не могат да бъдат заредени. Провери дали сървърът работи и опитай отново.';
        console.error('Грешка при зареждане на жанровете:', error);
    } finally {
        reloadButton.disabled = false;
    }
}
reloadButton.addEventListener('click', loadGenres);
loadGenres();
