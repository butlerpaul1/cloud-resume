///Call the api to retrieve the current user count
function getUserCount() {
    const url = 'api/visitorCounter'
    console.log('Calling: ' + url )
    fetch(url)
        .then(data => {
            return data.json()
        })
        .then(res => {
            let visitorCount = res;
            let myContainer = document.getElementById('visitorCount') as HTMLInputElement;
            myContainer.innerHTML = visitorCount;
        })
}