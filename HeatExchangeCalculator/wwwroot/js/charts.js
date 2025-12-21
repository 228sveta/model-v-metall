// Функция для создания графиков
function createTemperatureChart() {
    // Данные из таблицы
    const heights = [];
    const materialTemps = [];
    const gasTemps = [];
    const differences = [];
    
    // Берем данные из таблицы
    const table = document.querySelector('table');
    const rows = table.querySelectorAll('tbody tr');
    
    rows.forEach(row => {
        const cells = row.querySelectorAll('td');
        if (cells.length >= 9) {
            heights.push(cells[0].textContent.trim());
            materialTemps.push(parseFloat(cells[6].textContent));
            gasTemps.push(parseFloat(cells[7].textContent));
            differences.push(parseFloat(cells[8].textContent));
        }
    });
    
    // График 1: Температуры
    const ctx1 = document.getElementById('temperatureChart');
    if (ctx1 && materialTemps.length > 0) {
        new Chart(ctx1, {
            type: 'line',
            data: {
                labels: heights,
                datasets: [
                    {
                        label: 'Температура материала, °C',
                        data: materialTemps,
                        borderColor: 'red',
                        backgroundColor: 'rgba(255, 0, 0, 0.1)',
                        borderWidth: 2
                    },
                    {
                        label: 'Температура газа, °C',
                        data: gasTemps,
                        borderColor: 'blue',
                        backgroundColor: 'rgba(0, 0, 255, 0.1)',
                        borderWidth: 2
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    title: {
                        display: true,
                        text: 'Температуры материала и газа'
                    }
                },
                scales: {
                    y: {
                        title: {
                            display: true,
                            text: 'Температура, °C'
                        }
                    },
                    x: {
                        title: {
                            display: true,
                            text: 'Высота слоя, м'
                        }
                    }
                }
            }
        });
    }
    
    // График 2: Разность температур
    const ctx2 = document.getElementById('differenceChart');
    if (ctx2 && differences.length > 0) {
        new Chart(ctx2, {
            type: 'line',
            data: {
                labels: heights,
                datasets: [{
                    label: 'Разность температур, °C',
                    data: differences,
                    borderColor: 'green',
                    backgroundColor: 'rgba(0, 255, 0, 0.1)',
                    borderWidth: 2,
                    fill: true
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    title: {
                        display: true,
                        text: 'Разность температур Δt = t - T'
                    }
                },
                scales: {
                    y: {
                        title: {
                            display: true,
                            text: 'ΔT, °C'
                        }
                    },
                    x: {
                        title: {
                            display: true,
                            text: 'Высота слоя, м'
                        }
                    }
                }
            }
        });
    }
}

// Запускаем когда страница загрузится
window.onload = function() {
    createTemperatureChart();
};