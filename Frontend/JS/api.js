 async function loadLofiPlayer() {
    const playerContainer = document.getElementById('Lofi-container');
    const stateText = document.getElementById('lofi-description');

    stateText.textContent = "Lofi muziek wordt geladen..."

    try{
        const response = await fetch('https://api.mixcloud.com/DJCampbell/lo-fi-instrumental-hip-hop-3/');
        const data = await response.json();

        stateText.textContent = "Playing: " + data.name + " by " + data.user.name;

            const iframe = document.createElement('iframe');
            iframe.setAttribute('src', `https://www.mixcloud.com/widget/iframe/?hide_cover=1&feed=${encodeURIComponent(data.key)}`);
            iframe.style.width = "100%";
            iframe.style.height = "100px";
            iframe.style.border = "none";

            playerContainer.appendChild(iframe);
    }

    catch (error) {
        console.error('Error fetching data:', error);
        stateText.textContent = "Error fetching track information.";
    }
}

loadLofiPlayer();