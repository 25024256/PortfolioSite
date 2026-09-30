const projectData = [
    {
        title: "Trashtag",
        category: "Websites",
        lastUpdated: "December 6, 2025",
        description: "A web application designed to help users report and track litter in their community.",
        link: "https://github.com/25024256/TrashTag.git",
        image: "../Images/Trashtag-project.png"
    },
    {
        title: "HotelSimulator",
        category: "Java",
        lastUpdated: "July 26, 2026",
        description: "I completed the HotelSimulator project, which was a project coded in java and a great opportunity to practice my programming skills.",
        link: "https://github.com/hhs-se-semester-2-onderwijs/klas-3-groep-1.git"
    },
    {
        title: "Game of Life",
        category: "Websites",
        lastUpdated: "June 30, 2026",
        description: "This is a web application that simulates the Game of Life, a cellular automaton devised by mathematician John Conway.",
        link: "https://github.com/25024256/Game-of-Life.git"
    },
    {
        title: "Portfolio Website",
        category: "Websites",
        lastUpdated: "September 9, 2026",
        description: "This is my personal portfolio website which I coded myself, which showcases my skills, projects, and experiences as a web developer.",
        link: "https://github.com/25024256/PortfolioSite.git"
    }
];

function DynamicProjectlist(projectsArray) {
    const container = document.querySelector('.projects-posts');
    
    container.innerHTML = '';
    
    projectsArray.forEach(project => {

        const article = document.createElement('article');
        article.classList.add('projects-post');

        const titleElement = document.createElement('h3');
        titleElement.textContent = project.title;

        const pElement = document.createElement('p');
        const emElement = document.createElement('em');
        emElement.textContent = "Last updated: " + project.lastUpdated; // Plusje gebruikt!
        pElement.appendChild(emElement);

        const descElement = document.createElement('p');
        descElement.textContent = project.description;

        const linkElement = document.createElement('a');
        linkElement.href = project.link;
        linkElement.target = "_blank";
        linkElement.textContent = "View Project";
        linkElement.setAttribute('aria-label','View '+ project.title + 'on Github')

        article.appendChild(titleElement);
        article.appendChild(pElement);
        article.appendChild(descElement);
        article.appendChild(linkElement);

        if (project.image) {
            const imgElement = document.createElement('img');
            imgElement.src = project.image;
            imgElement.alt = "Image of " + project.title;
            imgElement.classList.add('trashtag-image');
            article.appendChild(imgElement);
        }

        container.appendChild(article);
    });
}

DynamicProjectlist(projectData);

function filterProjects(category) {
    
    if (category === 'All') {
        DynamicProjectlist(projectData);
    } else {
        
        const filteredData = projectData.filter(project => project.category === category);
        
        DynamicProjectlist(filteredData);
    }
}