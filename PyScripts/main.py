import os
import json
import matplotlib.pyplot as plt
import numpy as np

folder = "2025-11-16_08-46-45"
gen = 100

os.makedirs(f"value match/{folder}", exist_ok=True)
os.makedirs(f"value match/{folder}/dists", exist_ok=True)
os.makedirs(f"value match/{folder}/solspace", exist_ok=True)

def fitness_values():
  with open(f"../Assets/Value Match/Saves/{folder}/gen_{gen}.json", "r") as f:
    data = json.load(f)
  
  fitnessValues = data["averageFitnessValues"][:gen]

  xticks = list(range(0, 100, 5))
  if 0 not in xticks:
    xticks.append(0)
  if 100 not in xticks:
    xticks.append(100)
  yticks = np.linspace(0, 1, 21)
  ytickLabels = [f"{t:.1f}" if int(t * 100) % 10 == 0 else "" for t in yticks]

  plt.figure(figsize=(6, 3))
  plt.subplots_adjust(top=0.9, bottom=0.15, left=0.11, right=0.975)
  plt.plot(range(1, len(fitnessValues) + 1), fitnessValues, marker='.', markersize=1, linewidth=0.5)
  plt.xlabel("Nesil Numarası")
  plt.ylabel("Ortalama Uygunluk Değeri")
  plt.title(f"{gen} nesil boyunca Ortalama Uygunluk Değerleri")
  plt.grid(True, linestyle='--', alpha=0.5)
  plt.xticks(xticks)
  plt.yticks(yticks, ytickLabels)
  plt.xlim(1, gen)
  plt.ylim(0, 1)
  #plt.show()
  #return
  plt.savefig(f"value match/{folder}/mean_fitness_graph.png", dpi=300)
  plt.clf()
  plt.close()
  print(f"done average fitness graph over gens")

def dot_distribution():
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

    xticks = list(range(0, 256, 15))
    if 255 not in xticks:
      xticks.append(255)
    yticks = list(range(50, 1025, 100))
    if 0 not in yticks:
      yticks.append(0)
    if 1024 not in yticks:
      yticks.append(1024)
    plt.figure(figsize=(6, 3))
    plt.subplots_adjust(top=0.9, bottom=0.15, left=0.11, right=0.975)
    plt.scatter(x, y, s=1)
    plt.xlabel("Tahminler")
    plt.ylabel("Tahmin Sıklığı")
    plt.xticks(xticks)
    plt.yticks(yticks)
    plt.xlim(0, 255)
    plt.ylim(0, 1024)
    plt.grid(True, linestyle='--', alpha=0.5)
    plt.title(f"Popülasyon Nokta Dağılımı ({i}.Nesil)")
    #plt.show()
    #break
    plt.savefig(f"value match/{folder}/dists/{i}.png", dpi=300)
    plt.clf()
    plt.close()
    print(f"dot dist done [{i}]")

def solution_space():
  for i in range(1, gen + 1):
    with open(f"../Assets/Value Match/Saves/{folder}/gen_{i}.json", "r") as f:
      data = json.load(f)

    population = data["entities"]
    x = [p["actions"][0] for p in population]

    counts = {}
    for v in x:
        counts[v] = counts.get(v, 0) + 1

    plt.figure(figsize=(6, 1))
    plt.subplots_adjust(top=0.65, bottom=0.55, left=0.01, right=0.975)
    for v in counts.keys():
      alpha = counts[v] / 1024.0
      alpha = alpha * 0.95 + 0.05
      #plt.scatter(v, 0, s=10, marker='o', facecolors='black', edgecolors='none', alpha=alpha)
      plt.vlines(x=v, ymin=0, ymax=1, colors='black', alpha=alpha, linewidth=3)

    xticks = list(range(0, 256, 20))
    if 255 not in xticks:
      xticks.append(255)
    plt.xlabel("Tahminler")
    plt.xlim(0, 255)
    plt.xticks(xticks)
    plt.yticks([])
    plt.title(f"1 Boyutlu Çözüm Uzayı ({i}.Nesil)")
    #plt.show()
    #break
    plt.savefig(f"value match/{folder}/solspace/{i}.png", dpi=300)
    plt.clf()
    plt.close()
    print(f"sol space done [{i}]")   


fitness_values()
dot_distribution()
solution_space()
