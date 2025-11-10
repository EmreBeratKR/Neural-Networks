import os
import json
import matplotlib.pyplot as plt

folder = "2025-11-10_13-53-53"
gen = 100

os.makedirs(f"value match/{folder}", exist_ok=True)
os.makedirs(f"value match/{folder}/dists", exist_ok=True)

def fitness_values():
  with open(f"../Assets/Value Match/Saves/{folder}/gen_{gen}.json", "r") as f:
    data = json.load(f)
  
  fitnessValues = data["averageFitnessValues"][:gen]
  print(len(fitnessValues))
  plt.plot(range(1, len(fitnessValues) + 1), fitnessValues, marker='.')
  plt.xlabel("Nesil Numarası")
  plt.ylabel("Ortalama Uygunluk Değeri")
  plt.title(f"{gen} nesil boyunca Ortalama Uygunluk Değerleri")
  plt.grid(True)
  plt.xlim(1, gen)
  plt.ylim(0, 1)
  plt.savefig(f"value match/{folder}/mean_fitness_graph.png", dpi=300)
  plt.clf()
  print(f"done average fitness graph over gens")

def solution_space():
  for i in range(1, gen + 1):
    with open(f"../Assets/Value Match/Saves/{folder}/gen_{i}.json", "r") as f:
      data = json.load(f)

    population = data["entities"]
    values = [p["actions"][0] for p in population]

    counts = {}
    for v in values:
        counts[v] = counts.get(v, 0) + 1

    x, y = [], []
    for val, count in counts.items():
        for j in range(count):
            x.append(val)
            y.append(j)

    plt.scatter(x, y, s=10)
    plt.xlabel("Tahminler")
    plt.ylabel("Kromozom Sayısı")
    plt.xlim(0, 255)
    plt.ylim(0, 1024)
    plt.title(f"Popülasyon Nokta Dağılımı ({i}.Nesil)")
    plt.savefig(f"value match/{folder}/dists/{i}.png", dpi=300)
    plt.clf()
    print(f"dist done [{i}]")

fitness_values()
solution_space()
