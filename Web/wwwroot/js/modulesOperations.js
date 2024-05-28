// Чтобы окно редактирования и создания закрывалось, нужно нажать на затемнитель
const moduleShadow = document.getElementsByClassName("module-add-window-background-shadow");
if (moduleShadow.length !== 0) {
    moduleShadow[0].addEventListener('click', () => {
        document.getElementsByClassName("module-add-window")[0].hidden = true;
        // По всему фронту удаляем элементы, которые подключаются к форме ситуативно
        const deleteBtn = document.getElementsByClassName("delete-button");
        if (deleteBtn.length !== 0) {
            deleteBtn[0].remove();
        }
    });
}

// Кнопка добавления открывает окно с пустой формой
let addModuleButton = document.getElementById("add-module")
if (addModuleButton !== null) {
    addModuleButton.addEventListener('click', async () => {
        await openAddModuleWindow()
    })
}

// Само создание окна редактирования модуля
async function openModuleDetailsWindow(moduleId) {
    // Неавторизованному пользователю не даем редактировать и создавать
    let user = "";
    await fetch('/api/isAuthenticated', {method: 'get'})
        .then((response) => response.text())
        .then((userId) => {
            if (userId !== "false") {
                user = userId
            }
        });

    // Стучимся в API и получаем данные того модуля, с которым работаем
    const getModuleData = new URL('http://localhost:5039/api/get/module');
    getModuleData.search = new URLSearchParams(moduleId).toString();
    await fetch(getModuleData, { method: 'get' })
        .then((response) => response.json())
        .then(async (infoParams) => {
            if (user !== "") {
                // Заполняем форму действующими данными, чтобы сразу можно было их менять
                document.getElementById("module-uuid").value = infoParams["uuid"];
                document.getElementById("module-title").innerHTML = `<input type="text" id="module-title-input" class="window-input">`;
                document.getElementById("module-type").innerHTML = `<input type="text" id="module-type-input" class="window-input">`;
                document.getElementById("module-title-input").value = infoParams["title"];
                document.getElementById("module-type-input").value = infoParams["type"];

                // Добавляем кнопку удаления модуля (красивую)
                const deleteButton = document.createElement("button");
                deleteButton.setAttribute("class", "delete-button button-9");
                deleteButton.setAttribute("role", "button");
                deleteButton.innerText = "Удалить модуль";
                document.getElementsByClassName("module-add-window-form")[0].appendChild(deleteButton);
                deleteButton.addEventListener("click", () => {
                    const getModulesData = new URL('http://localhost:5039/api/delete/module');
                    getModulesData.search = new URLSearchParams(moduleId).toString();
                    fetch(getModulesData, { method: 'post', body: moduleId})
                        .then(() => {
                            window.location.reload();
                        });
                });

                // Вешаем триггер сохранения модуля по нажатию
                document.getElementById("save-module").addEventListener('click', async () => {
                    await fetch('/api/save/module', { method: 'post', body: buildModuleForm()});
                    window.location.reload();
                });

                // Показываем заранее созданную, но теперь уже дополненную, форму редактирования модуля
                document.getElementsByClassName("module-add-window")[0].hidden = false;
            }
        });
}

// Конфигурируем форму под создание модуля и показываем ее
async function openAddModuleWindow() {
    document.getElementById("module-title").innerHTML = `<input type="text" id="module-title-input" class="window-input">`;
    document.getElementById("module-type").innerHTML = `<input type="text" id="module-type-input" class="window-input">`;
    document.getElementById("save-module").addEventListener('click', async () => {
        await fetch('/api/save/module', { method: 'post', body: buildModuleForm()});
        window.location.reload();
    })

    document.getElementsByClassName("module-add-window")[0].hidden = false;
}

// Собираем данные из блоков в одну форму, которую можно отправлять в API
function buildModuleForm() {
    let formData = new FormData();
    formData.append('module-uuid', document.getElementById("module-uuid").value);
    formData.append('module-title', document.getElementById("module-title-input").value);
    formData.append('module-type', document.getElementById("module-type-input").value);
    return formData
}