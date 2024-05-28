document.addEventListener('DOMContentLoaded', async function () {
    // вешаем на названия ОП триггеры вызова окна редактирования (решил не делать кнопку, а вызывать меню по кликабельному имени)
    const programs = document.getElementsByClassName('eduProgram')
    for (let program of programs) {
        program.addEventListener('click', async () => {
            await openDetailsWindow(program.id)
        });
    }

    // Во всех формах есть кнопка сохранения, навесим на нее триггер сразу по загрузке страницы
    const saveBtn = document.getElementById("save-program");
    if (saveBtn !== null) {
        saveBtn.addEventListener('click', async () => {
            const body = buildProgramForm();
            if (body !== null) {
                await fetch('api/save/program', { method: 'post', body: body});
                window.location.reload();
            }
        });
    }
    
    // Меняем Guid всех институтов и ответственных лиц на читаемые имена.
    // Не менял саму сущность ОП, поскольку пришлось бы протаскивать изменения почти везде
    const instituteFields = document.getElementsByClassName("institute-table");
    for (let field of instituteFields) {
        const uuid = field.innerText
        const getInstituteTitle = new URL('http://localhost:5039/api/get/institute/title');
        getInstituteTitle.search = new URLSearchParams(uuid).toString();
        await fetch(getInstituteTitle, { method: 'get' })
            .then((response) => response.json())
            .then(async (title) => {
                field.innerText = title;
            });
    }

    const headFields = document.getElementsByClassName("head-table");
    for (let field of headFields) {
        const uuid = field.innerText
        const getHeadFullname = new URL('http://localhost:5039/api/get/head/fullname');
        getHeadFullname.search = new URLSearchParams(uuid).toString();
        await fetch(getHeadFullname, { method: 'get' })
            .then((response) => response.json())
            .then(async (title) => {
                field.innerText = title;
            });
    }

    // Делаем как и с ОП - каждый модуль кликабелен и вызывает панель управления
    const modules = document.getElementsByClassName('moduleTitle')
    for (let module of modules) {
        module.addEventListener('click', async () => {
            await openModuleDetailsWindow(module.id)
        });
    }
});